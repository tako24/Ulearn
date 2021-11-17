using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Reports
{
	public delegate string MakeCaption(string caption);
	public delegate string BeginList();
	public delegate string MakeItem(string valueType, string entry);
	public delegate string EndList();
	public delegate T MakeStatistics<out T>(IEnumerable<double> data);
	public class ReportMaker
	{
		public string MakeReport<T>(IEnumerable<Measurement> measurements, IFormatter format, IStatisticMaker<T> statistics)
		{
			var data = measurements.ToList();
			var result = new StringBuilder();
			result.Append(format.MakeCaption(statistics.Caption));
			result.Append(format.BeginList());
			result.Append(format.MakeItem("Temperature", statistics.MakeStatistics(data.Select(z => z.Temperature)).ToString()));
			result.Append(format.MakeItem("Humidity", statistics.MakeStatistics(data.Select(z => z.Humidity)).ToString()));
			result.Append(format.EndList());
			return result.ToString();
		}
	}

	public interface IFormatter
	{
		MakeCaption MakeCaption { get; }
		MakeItem MakeItem { get; }
		BeginList BeginList { get; }
		EndList EndList { get; }
	}

	public class HtmlFormat : IFormatter
	{
		public MakeCaption MakeCaption { get; }
		public MakeItem MakeItem { get; }
		public BeginList BeginList { get; }
		public EndList EndList { get; }
		public HtmlFormat()
		{
			MakeCaption = caption => $"<h1>{caption}</h1>";
			BeginList = () => "<ul>";
			MakeItem = (valueType, entry) => $"<li><b>{valueType}</b>: {entry}";
			EndList = () => "</ul>";
		}
	}

	public class MarkdownFormatter : IFormatter
	{
		public MakeCaption MakeCaption { get; }
		public MakeItem MakeItem { get; }
		public BeginList BeginList { get; }
		public EndList EndList { get; }
		public MarkdownFormatter()
		{
			MakeCaption = caption => $"## {caption}\n\n";
			BeginList = () => "";
			MakeItem = (valueType, entry) => $" * **{valueType}**: {entry}\n\n";
			EndList = () => "";
		}
	}

	public interface IStatisticMaker<T>
	{
		string Caption { get; }
		MakeStatistics<T> MakeStatistics { get; }
	}

	public class StatisticMaker : IStatisticMaker<double>
	{
		public string Caption { get; }
		public MakeStatistics<double> MakeStatistics { get; }
		public StatisticMaker()
		{
			Caption = "Median";
			MakeStatistics = (information) =>
			{
				var list = information.OrderBy(z => z).ToList();
				if (list.Count % 2 == 0)
					return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
				else
					return list[list.Count / 2];
			};
		}
	}

	public class MeanAndStdStatistics : IStatisticMaker<MeanAndStd>
	{
		public string Caption { get; }
		public MakeStatistics<MeanAndStd> MakeStatistics { get; }
		public MeanAndStdStatistics()
		{
			Caption = "Mean and Std";
			MakeStatistics = (information) =>
			{
				var dataList = information.ToList();
				var mean = dataList.Average();
				var std = Math.Sqrt(dataList.Select(z => Math.Pow(z - mean, 2)).Sum() / (dataList.Count - 1));

				return new MeanAndStd
				{
					Mean = mean,
					Std = std
				};
			};
		}
	}

	public static class ReportMakerHelper
	{
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data)
		{
			return new ReportMaker().MakeReport(data, new HtmlFormat(), new MeanAndStdStatistics());
		}


		public static string MedianMarkdownReport(IEnumerable<Measurement> data)
		{
			return new ReportMaker().MakeReport(data, new MarkdownFormatter(), new StatisticMaker());
		}

		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
		{
			return new ReportMaker().MakeReport(measurements, new MarkdownFormatter(), new MeanAndStdStatistics());
		}

		public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
		{
			return new ReportMaker().MakeReport(measurements, new HtmlFormat(), new StatisticMaker());
		}
	}
}