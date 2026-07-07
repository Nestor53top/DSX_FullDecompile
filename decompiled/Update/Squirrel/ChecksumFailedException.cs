using System;

namespace Squirrel;

internal class ChecksumFailedException : Exception
{
	public string Filename { get; set; }
}
