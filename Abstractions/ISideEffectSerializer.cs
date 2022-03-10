namespace Abstractions;

public interface ISideEffectSerializer
{
    byte[] Serialize(ISideEffect sideEffect);
    ISideEffect Deserialize(byte[] payload);
}