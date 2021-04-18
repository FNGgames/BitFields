# BitFields

Unmanaged structs for representing large collections of flags.<br>
Use this when the collection of flags is larger than the capacity of a single uint.<br>
Flag bits are stored in the binary bits of an unmanaged array of UInt32s.<br>
These structs provide a unified interface for performing bitwise operations on the flag collection.<br>

# API

## Bit Manipulation

Get / Set / Unset / Flip a _single bit_ using the index of that bit (e.g. `b.ToggleBit(5); b.SetBit(0,1,2);`)<br>
Get / Set / Unset / Flip _multiple bits_ using another bitfield as a mask (e.g. `b.SetBits(mask);`)<br>

## Bitwise Operators

Static bitwise operators ( & | ^ ~ << >> ) for bitwise arithmatic.<br>
Operators allocate new copies on the stack like any other value type.<br>

## Queries

IsEmpty() - Are all the bits set to zero?<br>
HasAllOF(BitField mask) - Are _ALL_ of the flags in the given mask set in the current object?<br>
HasAnyOf(BitField mask) - Are _ANY_ of the flags in the given mask set in the current object?<br>
HasNoneOf(BitField mask) - Are _NONE_ of the flags in the given mask set in the current object?<br>

## Indexing and Enumeration

Indexer `this[int index]` for indexing directly into the bits. (e.g. `b[3] = !b[4]`).<br>
Enumerable in C#7 or later (requires `ref struct` in the enumerator to allow pointer manipulation).<br>

## IEquatable

Fast comparison for use in hashtables, overloaded comparison operators ( == != ).<br>




