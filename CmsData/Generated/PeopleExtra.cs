using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData
{
	[Table(Name="dbo.PeopleExtra")]
	public partial class PeopleExtra : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private string _Field;
		
		private string _StrValue;
		
		private DateTime? _DateValue;
		
		private DateTime _TransactionTime;
		
   		
    	
		private EntityRef< Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnFieldChanging(string value);
		partial void OnFieldChanged();
		
		partial void OnStrValueChanging(string value);
		partial void OnStrValueChanged();
		
		partial void OnDateValueChanging(DateTime? value);
		partial void OnDateValueChanged();
		
		partial void OnTransactionTimeChanging(DateTime value);
		partial void OnTransactionTimeChanged();
		
    #endregion
		public PeopleExtra()
		{
			
			
			this._Person = default(EntityRef< Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL", IsPrimaryKey=true)]
		public int PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="Field", UpdateCheck=UpdateCheck.Never, Storage="_Field", DbType="varchar(50) NOT NULL", IsPrimaryKey=true)]
		public string Field
		{
			get { return this._Field; }

			set
			{
				if (this._Field != value)
				{
				
                    this.OnFieldChanging(value);
					this.SendPropertyChanging();
					this._Field = value;
					this.SendPropertyChanged("Field");
					this.OnFieldChanged();
				}

			}

		}

		
		[Column(Name="StrValue", UpdateCheck=UpdateCheck.Never, Storage="_StrValue", DbType="varchar(100)")]
		public string StrValue
		{
			get { return this._StrValue; }

			set
			{
				if (this._StrValue != value)
				{
				
                    this.OnStrValueChanging(value);
					this.SendPropertyChanging();
					this._StrValue = value;
					this.SendPropertyChanged("StrValue");
					this.OnStrValueChanged();
				}

			}

		}

		
		[Column(Name="DateValue", UpdateCheck=UpdateCheck.Never, Storage="_DateValue", DbType="datetime")]
		public DateTime? DateValue
		{
			get { return this._DateValue; }

			set
			{
				if (this._DateValue != value)
				{
				
                    this.OnDateValueChanging(value);
					this.SendPropertyChanging();
					this._DateValue = value;
					this.SendPropertyChanged("DateValue");
					this.OnDateValueChanged();
				}

			}

		}

		
		[Column(Name="TransactionTime", UpdateCheck=UpdateCheck.Never, Storage="_TransactionTime", DbType="datetime NOT NULL", IsPrimaryKey=true)]
		public DateTime TransactionTime
		{
			get { return this._TransactionTime; }

			set
			{
				if (this._TransactionTime != value)
				{
				
                    this.OnTransactionTimeChanging(value);
					this.SendPropertyChanging();
					this._TransactionTime = value;
					this.SendPropertyChanged("TransactionTime");
					this.OnTransactionTimeChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_PeopleExtra_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
		public Person Person
		{
			get { return this._Person.Entity; }

			set
			{
				Person previousValue = this._Person.Entity;
				if (((previousValue != value) 
							|| (this._Person.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Person.Entity = null;
						previousValue.PeopleExtras.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.PeopleExtras.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int);
						
					}

					this.SendPropertyChanged("Person");
				}

			}

		}

		
	#endregion
	
		public event PropertyChangingEventHandler PropertyChanging;
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
				this.PropertyChanging(this, emptyChangingEventArgs);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

   		
	}

}
