using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
	public ulong clientId;
	public int colorIndex;
	public FixedString64Bytes name;
	public FixedString64Bytes id;

	public bool Equals(PlayerData other)
	{
		return clientId == other.clientId
			&& colorIndex == other.colorIndex
			&& name.Equals(other.name)
			&& id.Equals(other.id);
	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref clientId);
		serializer.SerializeValue(ref colorIndex);
		serializer.SerializeValue(ref name);
		serializer.SerializeValue(ref id);
	}
}
