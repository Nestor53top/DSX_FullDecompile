using System;
using System.IO;
using Mono.Cecil.Cil;

namespace Mono.Cecil.Mdb;

public sealed class MdbWriterProvider : ISymbolWriterProvider
{
	public ISymbolWriter GetSymbolWriter(ModuleDefinition module, string fileName)
	{
		Mixin.CheckModule(module);
		Mixin.CheckFileName(fileName);
		return new MdbWriter(module.Mvid, fileName);
	}

	public ISymbolWriter GetSymbolWriter(ModuleDefinition module, Stream symbolStream)
	{
		throw new NotImplementedException();
	}
}
