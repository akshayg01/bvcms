using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
	[Table(Name="GetTotalContributions")]
	public partial class GetTotalContribution
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Cnt;
		
		private decimal? _Amt;
		
		private decimal? _Plg;
		
		private string _Fund;
		
		
		public GetTotalContribution()
		{
		}

		
		
		[Column(Name="Cnt", Storage="_Cnt", DbType="int")]
		public int? Cnt
		{
			get
			{
				return this._Cnt;
			}

			set
			{
				if (this._Cnt != value)
					this._Cnt = value;
			}

		}

		
		[Column(Name="Amt", Storage="_Amt", DbType="Decimal(38,2)")]
		public decimal? Amt
		{
			get
			{
				return this._Amt;
			}

			set
			{
				if (this._Amt != value)
					this._Amt = value;
			}

		}

		
		[Column(Name="Plg", Storage="_Plg", DbType="Decimal(38,2)")]
		public decimal? Plg
		{
			get
			{
				return this._Plg;
			}

			set
			{
				if (this._Plg != value)
					this._Plg = value;
			}

		}

		
		[Column(Name="Fund", Storage="_Fund", DbType="varchar(40) NOT NULL")]
		public string Fund
		{
			get
			{
				return this._Fund;
			}

			set
			{
				if (this._Fund != value)
					this._Fund = value;
			}

		}

		
    }

}
