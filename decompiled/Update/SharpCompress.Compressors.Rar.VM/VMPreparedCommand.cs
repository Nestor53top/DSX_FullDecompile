namespace SharpCompress.Compressors.Rar.VM;

internal class VMPreparedCommand
{
	internal VMCommands OpCode { get; set; }

	internal bool IsByteMode { get; set; }

	internal VMPreparedOperand Op1 { get; }

	internal VMPreparedOperand Op2 { get; }

	internal VMPreparedCommand()
	{
		Op1 = new VMPreparedOperand();
		Op2 = new VMPreparedOperand();
	}
}
