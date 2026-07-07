namespace Polly.Caching;

public interface ICacheItemSerializer<TResult, TSerialized>
{
	TSerialized Serialize(TResult objectToSerialize);

	TResult Deserialize(TSerialized objectToDeserialize);
}
