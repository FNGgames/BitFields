# BitFields

Unmanaged structs for representing large collections of flags and performing bitwise arithmetic on them.<br>
Flag bits are stored in the binary bits of an unmanaged array of UInt32s.<br>
No managed memory is allocated by these structs.

# API

## Bit Manipulation

Get / Set / Unset / Flip a _single bit_ using the index of that bit.<br>
The operations mutate the data in this struct, usual caveats for mutable value-types apply.<br>

`GetBit(int index);`<br>
`SetBit(int index);`<br>
`UnsetBit(int index);`<br>
`FlipBit(int index);`<br>

Get / Set / Unset / Flip _multiple bits_ using another bitfield as a mask<br>
The operations mutate the data in this struct, usual caveats for mutable value-types apply.<br>

`GetBits(BitField mask);`<br>
`SetBits(BitField mask);`<br>
`UnsetBits(BitField mask);`<br>
`FlipBits(BitField mask);`<br>

## Bitwise Operators

Static bitwise operators ( `& | ^ ~ << >>` ) for bitwise arithmetic.<br>
Operators allocate new copies on the stack like any other value type.<br>
Operations are O(n) where n is the number of words that make up the bit field.<br>

## Queries

`IsEmpty()` - Are all the bits set to zero?<br>
`HasAllOF(BitField mask)` - Are _ALL_ of the flags in the given mask set in the current object?<br>
`HasAnyOf(BitField mask)` - Are _ANY_ of the flags in the given mask set in the current object?<br>
`HasNoneOf(BitField mask)` - Are _NONE_ of the flags in the given mask set in the current object?<br>

## Indexing and Enumeration

Indexer `this[int index]` for indexing directly into the bits. (e.g. `b[3] = !b[4]`).<br>
Set operations mutate the data in this struct, usual caveats for mutable value-types apply.<be>
Enumerable in C#7 or later.<br>
It is safe to alter the structure while enumerating.<br>

## IEquatable

Fast comparison for use in hashtables, overloaded comparison operators ( == != ).<br>




