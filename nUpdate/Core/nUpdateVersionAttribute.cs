using System;

namespace nUpdate.Core
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class nUpdateVersionAttribute : Attribute
	{
		private readonly string nUpdateVersionString;

		public string VersionString
		{
			get { return nUpdateVersionString; }
		}

		public nUpdateVersionAttribute(string nUpdateVersionString)
		{
			this.nUpdateVersionString = nUpdateVersionString;
		}
	}
}
