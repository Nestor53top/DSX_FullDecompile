namespace ModernWpf.Controls;

public interface IKeyIndexMapping
{
	string KeyFromIndex(int index);

	int IndexFromKey(string key);
}
