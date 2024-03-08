﻿namespace UnoSpySnoopDebugger.IpcCommunicationContext;

public record GetElementPropertyRequest(string ElementToken);
public record GetElementPropertyResponse(List<DependencyPropertyInfo> DependencyPropertyInfoList);