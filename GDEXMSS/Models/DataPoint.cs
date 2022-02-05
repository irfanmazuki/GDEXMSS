using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GDEXMSS.Models
{
	//DataContract for Serializing Data - required to serve in JSON format
	[DataContract]
	public class DataPoint
	{
		public DataPoint(string x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		//Explicitly setting the name to be used while serializing to JSON.
		[DataMember(Name = "label")]
		public string X = null;

		//Explicitly setting the name to be used while serializing to JSON.
		[DataMember(Name = "y")]
		public Nullable<int> Y = null;
	}
}