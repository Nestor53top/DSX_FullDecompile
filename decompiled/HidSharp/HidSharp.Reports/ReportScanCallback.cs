namespace HidSharp.Reports;

public delegate void ReportScanCallback(byte[] buffer, int bitOffset, DataItem dataItem, int indexOfDataItem);
