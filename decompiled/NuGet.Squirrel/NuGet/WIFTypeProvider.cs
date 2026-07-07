using System;
using System.ServiceModel;

namespace NuGet;

internal abstract class WIFTypeProvider
{
	private sealed class WIFTypes40 : WIFTypeProvider
	{
		public override Type ChannelFactory => Type.GetType(QualifyTypeName("Microsoft.IdentityModel.Protocols.WSTrust.WSTrustChannelFactory"));

		public override Type RequestSecurityToken => Type.GetType(QualifyTypeName("Microsoft.IdentityModel.Protocols.WSTrust.RequestSecurityToken"));

		public override Type EndPoint => typeof(EndpointAddress);

		public override Type RequestTypes => Type.GetType(QualifyTypeName("Microsoft.IdentityModel.SecurityTokenService.RequestTypes"));

		public override Type KeyTypes => Type.GetType(QualifyTypeName("Microsoft.IdentityModel.SecurityTokenService.KeyTypes"));

		protected override string AssemblyName => "Microsoft.IdentityModel, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
	}

	private sealed class WIFTypes45 : WIFTypeProvider
	{
		public override Type ChannelFactory => Type.GetType("System.ServiceModel.Security.WSTrustChannelFactory, System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

		public override Type RequestSecurityToken => Type.GetType(QualifyTypeName("System.IdentityModel.Protocols.WSTrust.RequestSecurityToken"));

		public override Type EndPoint => Type.GetType(QualifyTypeName("System.IdentityModel.Protocols.WSTrust.EndpointReference"));

		public override Type RequestTypes => Type.GetType(QualifyTypeName("System.IdentityModel.Protocols.WSTrust.RequestTypes"));

		public override Type KeyTypes => Type.GetType(QualifyTypeName("System.IdentityModel.Protocols.WSTrust.KeyTypes"));

		protected override string AssemblyName => "System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
	}

	public abstract Type ChannelFactory { get; }

	public abstract Type RequestSecurityToken { get; }

	public abstract Type EndPoint { get; }

	public abstract Type RequestTypes { get; }

	public abstract Type KeyTypes { get; }

	protected abstract string AssemblyName { get; }

	public static WIFTypeProvider GetWIFTypes()
	{
		WIFTypeProvider wIFTypeProvider = new WIFTypes45();
		if (wIFTypeProvider.ChannelFactory != null)
		{
			return wIFTypeProvider;
		}
		wIFTypeProvider = new WIFTypes40();
		if (wIFTypeProvider.ChannelFactory != null)
		{
			return wIFTypeProvider;
		}
		return null;
	}

	protected string QualifyTypeName(string typeName)
	{
		return typeName + "," + AssemblyName;
	}
}
