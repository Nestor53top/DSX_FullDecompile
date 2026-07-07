using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal abstract class Transform
{
	private MissingTargetMessage missingTargetMessage;

	private bool applyTransformToAllTargetNodes;

	private bool useParentAsTargetNode;

	private XmlTransformationLogger logger;

	private XmlElementContext context;

	private XmlNode currentTransformNode;

	private XmlNode currentTargetNode;

	private string argumentString;

	private IList<string> arguments;

	protected bool ApplyTransformToAllTargetNodes
	{
		get
		{
			return applyTransformToAllTargetNodes;
		}
		set
		{
			applyTransformToAllTargetNodes = value;
		}
	}

	protected bool UseParentAsTargetNode
	{
		get
		{
			return useParentAsTargetNode;
		}
		set
		{
			useParentAsTargetNode = value;
		}
	}

	protected MissingTargetMessage MissingTargetMessage
	{
		get
		{
			return missingTargetMessage;
		}
		set
		{
			missingTargetMessage = value;
		}
	}

	protected XmlNode TransformNode
	{
		get
		{
			if (currentTransformNode == null)
			{
				return context.TransformNode;
			}
			return currentTransformNode;
		}
	}

	protected XmlNode TargetNode
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			if (currentTargetNode == null)
			{
				{
					IEnumerator enumerator = TargetNodes.GetEnumerator();
					try
					{
						if (enumerator.MoveNext())
						{
							return (XmlNode)enumerator.Current;
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			return currentTargetNode;
		}
	}

	protected XmlNodeList TargetNodes
	{
		get
		{
			if (UseParentAsTargetNode)
			{
				return context.TargetParents;
			}
			return context.TargetNodes;
		}
	}

	protected XmlNodeList TargetChildNodes => context.TargetNodes;

	protected XmlTransformationLogger Log
	{
		get
		{
			if (logger == null)
			{
				logger = context.GetService<XmlTransformationLogger>();
				if (logger != null)
				{
					logger.CurrentReferenceNode = (XmlNode)(object)context.TransformAttribute;
				}
			}
			return logger;
		}
	}

	protected string ArgumentString => argumentString;

	protected IList<string> Arguments
	{
		get
		{
			if (arguments == null && argumentString != null)
			{
				arguments = XmlArgumentUtility.SplitArguments(argumentString);
			}
			return arguments;
		}
	}

	private string TransformNameLong
	{
		get
		{
			if (context.HasLineInfo)
			{
				return string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_TransformNameFormatLong, new object[3] { TransformName, context.TransformLineNumber, context.TransformLinePosition });
			}
			return TransformNameShort;
		}
	}

	internal string TransformNameShort => string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_TransformNameFormatShort, new object[1] { TransformName });

	private string TransformName => GetType().Name;

	protected Transform()
		: this(TransformFlags.None)
	{
	}

	protected Transform(TransformFlags flags)
		: this(flags, MissingTargetMessage.Warning)
	{
	}

	protected Transform(TransformFlags flags, MissingTargetMessage message)
	{
		missingTargetMessage = message;
		applyTransformToAllTargetNodes = (flags & TransformFlags.ApplyTransformToAllTargetNodes) == TransformFlags.ApplyTransformToAllTargetNodes;
		useParentAsTargetNode = (flags & TransformFlags.UseParentAsTargetNode) == TransformFlags.UseParentAsTargetNode;
	}

	protected abstract void Apply();

	protected T GetService<T>() where T : class
	{
		return context.GetService<T>();
	}

	internal void Execute(XmlElementContext context, string argumentString)
	{
		if (this.context != null || this.argumentString != null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		try
		{
			this.context = context;
			this.argumentString = argumentString;
			arguments = null;
			if (ShouldExecuteTransform())
			{
				flag2 = true;
				Log.StartSection(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformBeginExecutingMessage, TransformNameLong);
				Log.LogMessage(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformStatusXPath, context.XPath);
				if (ApplyTransformToAllTargetNodes)
				{
					ApplyOnAllTargetNodes();
				}
				else
				{
					ApplyOnce();
				}
			}
		}
		catch (Exception ex)
		{
			flag = true;
			if (context.TransformAttribute != null)
			{
				Log.LogErrorFromException(XmlNodeException.Wrap(ex, (XmlNode)(object)context.TransformAttribute));
			}
			else
			{
				Log.LogErrorFromException(ex);
			}
		}
		finally
		{
			if (flag2)
			{
				if (flag)
				{
					Log.EndSection(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformErrorExecutingMessage, TransformNameShort);
				}
				else
				{
					Log.EndSection(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformEndExecutingMessage, TransformNameShort);
				}
			}
			else
			{
				Log.LogMessage(MessageType.Normal, SR.XMLTRANSFORMATION_TransformNotExecutingMessage, TransformNameLong);
			}
			this.context = null;
			this.argumentString = null;
			arguments = null;
			ReleaseLogger();
		}
	}

	private void ReleaseLogger()
	{
		if (logger != null)
		{
			logger.CurrentReferenceNode = null;
			logger = null;
		}
	}

	private bool ApplyOnAllTargetNodes()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		bool result = false;
		XmlNode transformNode = TransformNode;
		foreach (XmlNode targetNode in TargetNodes)
		{
			XmlNode val = targetNode;
			try
			{
				currentTargetNode = val;
				currentTransformNode = transformNode.Clone();
				ApplyOnce();
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex);
				result = true;
			}
		}
		currentTargetNode = null;
		return result;
	}

	private void ApplyOnce()
	{
		WriteApplyMessage(TargetNode);
		Apply();
	}

	private void WriteApplyMessage(XmlNode targetNode)
	{
		IXmlLineInfo val = (IXmlLineInfo)(object)((targetNode is IXmlLineInfo) ? targetNode : null);
		if (val != null)
		{
			Log.LogMessage(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformStatusApplyTarget, targetNode.Name, val.LineNumber, val.LinePosition);
		}
		else
		{
			Log.LogMessage(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformStatusApplyTargetNoLineInfo, targetNode.Name);
		}
	}

	private bool ShouldExecuteTransform()
	{
		return HasRequiredTarget();
	}

	private bool HasRequiredTarget()
	{
		bool flag = false;
		bool existedInOriginal = false;
		if (!((!UseParentAsTargetNode) ? context.HasTargetNode(out var failedContext, out existedInOriginal) : context.HasTargetParent(out failedContext, out existedInOriginal)))
		{
			HandleMissingTarget(failedContext, existedInOriginal);
			return false;
		}
		return true;
	}

	private void HandleMissingTarget(XmlElementContext matchFailureContext, bool existedInOriginal)
	{
		string format = (existedInOriginal ? SR.XMLTRANSFORMATION_TransformSourceMatchWasRemoved : SR.XMLTRANSFORMATION_TransformNoMatchingTargetNodes);
		string message = string.Format(CultureInfo.CurrentCulture, format, new object[1] { matchFailureContext.XPath });
		switch (MissingTargetMessage)
		{
		case MissingTargetMessage.None:
			Log.LogMessage(MessageType.Verbose, message);
			break;
		case MissingTargetMessage.Information:
			Log.LogMessage(MessageType.Normal, message);
			break;
		case MissingTargetMessage.Warning:
			Log.LogWarning(matchFailureContext.Node, message);
			break;
		case MissingTargetMessage.Error:
			throw new XmlNodeException(message, matchFailureContext.Node);
		}
	}
}
