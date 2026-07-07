using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HidSharp.Reports.Input;

namespace HidSharp.Reports;

public class DeviceItem : DescriptorCollectionItem
{
	private sealed class ReportCollectionItemReports : Collection<Report>
	{
		private DeviceItem _item;

		public ReportCollectionItemReports(DeviceItem item)
		{
			_item = item;
		}

		protected override void ClearItems()
		{
			using (IEnumerator<Report> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Report current = enumerator.Current;
					current.DeviceItem = null;
				}
			}
			base.ClearItems();
		}

		protected override void InsertItem(int index, Report item)
		{
			Throw.If.Null(item).False(item.DeviceItem == null);
			item.DeviceItem = _item;
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			base[index].DeviceItem = null;
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, Report item)
		{
			throw new NotImplementedException();
		}
	}

	private ReportCollectionItemReports _reports;

	public IList<Report> Reports => _reports;

	public IEnumerable<Report> InputReports => Reports.Where((Report report) => report.ReportType == ReportType.Input);

	public IEnumerable<Report> OutputReports => Reports.Where((Report report) => report.ReportType == ReportType.Output);

	public IEnumerable<Report> FeatureReports => Reports.Where((Report report) => report.ReportType == ReportType.Feature);

	public DeviceItem()
	{
		_reports = new ReportCollectionItemReports(this);
	}

	public DeviceItemInputParser CreateDeviceItemInputParser()
	{
		return new DeviceItemInputParser(this);
	}
}
