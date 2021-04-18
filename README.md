# BitFields

Unmanaged structs for representing large collections of flags.
Use this when the collection of flags is larger than the capacity of a single uint.
Flag bits are stored in the binary bits of an unmanaged array of UInt32s.
These structs provide a unified interface for performing bitwise operations on the flag collection.

# API

## Bit Manipulation

Get / Set / Unset / Flip a _single bit_ using the index of that bit (e.g. `b.ToggleBit(5); b.SetBit(0,1,2);`)
Get / Set / Unset / Flip _multiple bits_ using another bitfield as a mask (e.g. `b.SetBits(mask);`)

## Bitwise Operators

Static bitwise operators ( & | ^ ~ << >> ) are implemented for performing custom bitwise arithmatic.
These operators allocate new bitfields on the stack and return them, as if they were any other value type.

## Queries

IsEmpty() - Are all the bits set to zero?
HasAllOF(BitField mask) - Are _ALL_ of the flags in the given mask set in the current object?
HasAnyOf(BitField mask) - Are _ANY_ of the flags in the given mask set in the current object?
HasNoneOf(BitField mask) - Are _NONE_ of the flags in the given mask set in the current object?

## Indexing and Enumeration

Indexer `this[int index]` for indexing directly into the bits. (e.g. `b[1] = true; b[3] = !b[4]`).
Enumerable in C#7 or later (requires `ref struct` in the inumerator to allow pointer manipulation).

## IEquatable

Fast comparison for use in hashtables, overloaded comparison operators ( == != ).




