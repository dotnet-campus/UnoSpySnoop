using System.Collections.ObjectModel;
using System.Diagnostics;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Threading;
using dotnetCampus.Ipc.Utils.Logging;
using Microsoft.UI.Xaml.Data;

using UnoSpySnoopDebugger.Communications;
using UnoSpySnoopDebugger.IpcCommunicationContext;
using UnoSpySnoopDebugger.Models;
using UnoSpySnoopDebugger.View;
using LogLevel = dotnetCampus.Ipc.Utils.Logging.LogLevel;

namespace UnoSpySnoopDebugger;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        var currentProcess = Process.GetCurrentProcess();
        var name = $"UnoSpySnoopDebugger_{currentProcess.ProcessName}_{currentProcess.Id}";
        var ipcProvider = new IpcProvider(name, new IpcConfiguration()
        {
            IpcTaskScheduling = IpcTaskScheduling.LocalOneByOne,
            IpcClientPipeConnector = new UnoSpySnoopDebuggerIpcClientPipeConnector(),
            IpcLoggerProvider = s => new CustomIpcLogger(s)
        });
        var jsonIpcDirectRoutedProvider = new JsonIpcDirectRoutedProvider(ipcProvider);
        IpcProvider = jsonIpcDirectRoutedProvider;

        _ = RefreshProcessInfoList();

#if HAS_UNO
        UnoSpySnoop.SpySnoop.StartSpyUI(SnoopRootGrid);
#endif
    }

    public ObservableCollection<CandidateDebugProcessInfo> ProcessInfoList { get; } =
        new ObservableCollection<CandidateDebugProcessInfo>();

    private async Task RefreshProcessInfoList()
    {
        ProcessInfoList.Clear();

        var processes = Process.GetProcesses().ToList();

        foreach (Process process in processes)
        {
            Console.WriteLine($"Process: {process.ProcessName}");
        }

#if DEBUG
        var currentProcess = Process.GetCurrentProcess();
        var otherInstance = processes.FirstOrDefault(p => p.Id != currentProcess.Id && p.ProcessName == currentProcess.ProcessName);
        if (otherInstance != null)
        {
            processes.Remove(otherInstance);
            processes.Insert(0, otherInstance);
        }

#if !MACCATALYST
        Window.Current.Title = $"UnoSpySnoopDebugger - PID:{currentProcess.Id}";
#endif

#endif

        foreach (Process process in processes)
        {
            await PeekProcess(process);
        }

        //await Parallel.ForEachAsync(processes, async (process, _) => { await PeekProcess(process); });
    }

    private async Task PeekProcess(Process process)
    {
        var peerName = $"UnoSpySnoop_{process.ProcessName}_{process.Id}";
        Console.WriteLine($"Try peek {peerName}");

        try
        {
            JsonIpcDirectRoutedClientProxy client =
                await IpcProvider.GetAndConnectClientAsync(peerName);
            var response = await client.GetResponseAsync<HelloResponse>(RoutedPathList.Hello);
            if (response is null)
            {
                return;
            }

            if (response.SnoopVersionText != VersionInfo.VersionText)
            {
                return;
            }

            var info = new CandidateDebugProcessInfo()
            {
                Client = client,
                CommandLine = response.CommandLine,
                ProcessId = response.ProcessId.ToString(),
                ProcessName = response.ProcessName,
            };

            DispatcherQueue.TryEnqueue(() => { ProcessInfoList.Add(info); });
        }
        catch (IpcClientPipeConnectionException e)
        {
            // Connection Fail
            Console.WriteLine($"Connection Fail {peerName}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public JsonIpcDirectRoutedProvider IpcProvider { get; set; }

    private async void StartDebugButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ProcessInfoListView.SelectedItem is CandidateDebugProcessInfo info)
        {
            if (info.Client is null)
            {
                return;
            }

#if !MACCATALYST
            Window.Current.Title = $"UnoSpySnoopDebugger - Debugging {info.ProcessName} PID:{info.ProcessId}";
#endif

            var snoopUserControl = new SnoopUserControl(info.Client);
            await snoopUserControl.StartAsync();

            RootGrid.Children.Clear();
            RootGrid.RowDefinitions.Clear();
            RootGrid.Children.Add(snoopUserControl);
        }
    }

    private void RefreshProcessInfoListButton_OnClick(object sender, RoutedEventArgs e)
    {
        _ = RefreshProcessInfoList();
    }
}

public class NotNullToIsEnableConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, string language)
    {
        return value is not null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }
}

class CustomIpcLogger : IpcLogger
{
    public CustomIpcLogger(string name) : base(name)
    {
    }

    protected override bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    protected override void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Console.WriteLine($"[IpcLogger]{logLevel} {formatter(state, exception)}");
    }
}
