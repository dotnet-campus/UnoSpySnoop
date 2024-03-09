# UnoSpySnoop

UnoSpySnoop ��һ�������������� Skia ƽ̨�µ� UNO Ӧ�� UI ����Ĺ���

## Ϊʲô��Ҫ�˹���

��Ϊ�� Skia ƽ̨�£������� WPF ���� GTK ������һ�� Surface ����Ⱦ���档��͵�����ԭ���� WPF �� UI ���Թ��ߣ��� SnoopWpf �ȹ��ߣ���ֻ�ܿ���һ��ͼƬ�����ܻ�ȡ��ȷ�Ľ���ṹ��ͨ�� UnoSpySnoop ���Ժܺõ��ڻ��� Skia ������ƽ̨���� Skia.Wpf �� Skia.Gtk �ϣ����и������濪�����ԣ���߿����ߵĽ��濪��Ч�ʣ��ر��ǵ����� Linux �����ϵ� Skia.Gtk Ӧ�õ�ʱ��

## ʹ�÷���

��׼�������� UI �������Ŀ���棬�������µ�׼����������

1. ��װ��Ϊ UnoSpySnoopProvider �� NuGet ��
1. ������ UI ���������һ����Ϊ SnoopRootGrid �� Grid �ؼ������ں�����ʾ���������벻Ҫ�� SnoopRootGrid ��������κ�ҵ���߼����棬��Ϊ�� SnoopRootGrid �����ݽ��ᱻ�������
1. ʹ�� UnoSpySnoop �����ռ��µ� SpySnoop ��̬���͵� StartSpyUI ���������� SnoopRootGrid ��Ϊ�������������׼��������ʾ����������

```csharp
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        UnoSpySnoop.SpySnoop.StartSpyUI(SnoopRootGrid);
    }
}
```

�������׼������֮�󣬼���������Ŀ������ UnoSpySnoopDebugger ���ߣ�ѡ�����е���Ŀ��Ȼ���� `Start UI Spy` ��ť�����ɽ��е��� UI �Ľ���

![](./Docs/Images/SelectDebugProcess.png)

## ��л

��л https://github.com/snoopwpf/snoopwpf �����ṩ�������Դ�Լ� UI ���沼��