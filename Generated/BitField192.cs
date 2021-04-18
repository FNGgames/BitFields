using System;
#pragma warning disable 649

namespace BitFields
{
    /// <summary>
    /// Unmanaged 192 bit struct used to represent a collection of flags.
    /// All standard bitwise operators are implemented ( &amp; | ^ ~ &lt;&lt; &gt;&gt; )
    /// </summary>
    public unsafe struct BitField192 : IEquatable<BitField192>
    {
        // PROPERTIES AND FIELDS

        #region DATA

        /// <summary>
        /// Number of words that make up the bit field
        /// </summary>
        public const int wordCount = 6;

        /// <summary>
        /// Number of bits in each word 
        /// </summary>
        public const int wordLength = 32;

        /// <summary>
        /// Total number of bits stored in this bitfield
        /// </summary>
        public const int bitCount = wordLength * wordCount;

        /// <summary>
        /// Unmanaged array of uint32 words that make up the bitfield
        /// </summary>
        private fixed uint words[wordCount];

        #endregion

        // PROPERTIES

        #region PROPERTIES

        /// <summary>
        /// The default bitfield (all zeros)
        /// </summary>
        public static BitField192 none => new BitField192();

        /// <summary>
        /// A new bitfield with all bits set to 1
        /// </summary>
        public static BitField192 all => ~new BitField192();

        #endregion

        // BIT MANIPULATION

        #region BIT_MANIPULATION

        /// <summary>
        /// Sets a single bit to 1
        /// </summary>
        /// 
        /// <param name="index">
        /// Index of the bit to set. Throws if out of range [0:bitCount].
        /// </param>
        public void SetBit(int index)
        {
            IndexInBounds(index);
            GetMask(index, out var wordIndex, out var mask);
            words[wordIndex] |= mask;
        }

        /// <summary>
        /// Sets a single bit to 0
        /// </summary>
        /// 
        /// <param name="index">
        /// Index of the bit to set. Throws if out of range [0:bitCount].
        /// </param>
        public void UnsetBit(int index)
        {
            IndexInBounds(index);
            GetMask(index, out var wordIndex, out var mask);
            words[wordIndex] &= ~mask;
        }

        /// <summary>
        /// Flips a single bit
        /// </summary>
        /// 
        /// <param name="index">
        /// Index of the bit to flip. Throws if out of range [0:bitCount].
        /// </param>
        public void FlipBit(int index)
        {
            IndexInBounds(index);
            GetMask(index, out var wordIndex, out var mask);
            words[wordIndex] ^= mask;
        }

        /// <summary>
        /// Sets all the bits at the supplied indices 1
        /// </summary>
        /// 
        /// <param name="indices">
        /// Array of bit indices. Throws if any are out of range [0:bitCount].
        /// </param>
        public void SetBits(params int[] indices)
        {
            foreach (var i in indices) SetBit(i);
        }

        /// <summary>
        /// Sets all the bits that match a mask to 1 ( bits |= mask )
        /// </summary>
        /// 
        /// <param name="mask">
        /// Mask of bits to set
        /// </param>
        public void SetBits(in BitField192 mask) => Or(in mask);

        /// <summary>
        /// Sets all the bits that match a mask to 0 ( bits &amp;= ~mask )
        /// </summary>
        /// 
        /// <param name="mask">
        /// Mask of bits to unset
        /// </param>
        public void UnsetBits(in BitField192 mask) => AndNot(in mask);

        /// <summary>
        /// Sets all the bits at the supplied indices 0
        /// </summary>
        /// 
        /// <param name="indices">
        /// Array of bit indices. Throws if any are out of range [0:bitCount].
        /// </param>
        public void UnsetBits(params int[] indices)
        {
            foreach (var i in indices) UnsetBit(i);
        }

        /// <summary>
        /// Flips all the bits that match a mask ( bits ^= mask )
        /// </summary>
        /// 
        /// <param name="mask">
        /// Mask of bits to flip
        /// </param>
        public void FlipBits(in BitField192 mask) => XOr(in mask);

        /// <summary>
        /// Flips all the bits at the supplied indices
        /// </summary>
        /// 
        /// <param name="indices">
        /// Array of bit indices. Throws if any are out of range [0:bitCount].
        /// </param>
        public void FlipBits(params int[] indices)
        {
            foreach (var i in indices) FlipBit( i);
        }

        #endregion

        // QUERIES

        #region QUERIES

        /// <summary>
        /// Gets the state of a single bit
        /// </summary>
        /// 
        /// <param name="index">
        /// Index of the bit. Throws if index out of range [0:bitCount].
        /// </param>
        public bool GetBit(int index)
        {
            IndexInBounds(index);
            GetMask(index, out var wordIndex, out var mask);
            return (words[wordIndex] & mask) == mask;
        }

        /// <summary>
        /// Determines if all bits are set to 0
        /// </summary>
        public bool IsEmpty()
        {
            for (var i = 0; i < wordCount; i++)
                if (words[i] != 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Determines if ALL of the bits from a given mask are set 
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public bool HasAllOf(in BitField192 mask) => (this & mask) == mask;

        /// <summary>
        /// Determines if ANY of the bits from a given mask are set 
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public bool HasAnyOf(in BitField192 mask) => !(this & mask).IsEmpty();

        /// <summary>
        /// Determines if NONE of the bits from a given mask are set 
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public bool HasNoneOf(in BitField192 mask) => (this & mask).IsEmpty();

        #endregion

        // BOOLEAN OPERATIONS

        #region BOOLEAN_OPERATIONS

        // And, Not, Or and XOr are the same operations performed element-by-element on the underlying uints

        /// <summary>
        /// Performs the bitwise boolean operation AND (bits &amp; mask)
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public void And(in BitField192 mask)
        {
            for (var i = 0; i < wordCount; i++)
                words[i] &= mask.words[i];
        }

        /// <summary>
        /// Performs the bitwise boolean operation AND-NOT (bits &amp; ~mask)
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public void AndNot(in BitField192 mask)
        {
            for (var i = 0; i < wordCount; i++)
                words[i] &= ~mask.words[i];
        }

        /// <summary>
        /// Performs the bitwise boolean operation NAND ~(bits &amp; mask)
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public void Nand(in BitField192 mask)
        {
            for (var i = 0; i < wordCount; i++)
                words[i] = ~(words[i] & mask.words[i]);
        }

        /// <summary>
        /// Performs the bitwise boolean operation OR (bits | mask)
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public void Or(in BitField192 mask)
        {
            for (var i = 0; i < wordCount; i++)
                words[i] |= mask.words[i];
        }

        /// <summary>
        /// Performs the bitwise boolean operation OR-NOT (bits | ~mask)
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public void OrNot(in BitField192 mask)
        {
            for (var i = 0; i < wordCount; i++)
                words[i] |= ~mask.words[i];
        }

        /// <summary>
        /// Performs the bitwise boolean operation NOR ~(bits | mask)
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public void Nor(in BitField192 mask)
        {
            for (var i = 0; i < wordCount; i++)
                words[i] = ~(words[i] | mask.words[i]);
        }


        /// <summary>
        /// Performs the bitwise boolean operation XOR (bits ^ mask)
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public void XOr(in BitField192 mask)
        {
            for (var i = 0; i < wordCount; i++)
                words[i] ^= mask.words[i];
        }

        /// <summary>
        /// Performs the bitwise boolean operation XOR-NOT (bits ^ ~mask)
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public void XOrNot(in BitField192 mask)
        {
            for (var i = 0; i < wordCount; i++)
                words[i] ^= ~mask.words[i];
        }

        /// <summary>
        /// Performs the bitwise boolean operation NOT-XOR ~(bits ^ ~mask)
        /// </summary>
        /// 
        /// <param name="mask">
        /// Query mask
        /// </param>
        public void NotXOr(in BitField192 mask)
        {
            for (var i = 0; i < wordCount; i++)
                words[i] = ~(words[i] ^ mask.words[i]);
        }

        /// <summary>
        /// Performs the bitwise boolean operation NOT (~bits)
        /// </summary>
        public void Not()
        {
            for (var i = 0; i < wordCount; i++)
                words[i] = ~words[i];
        }

        /// <summary>
        /// Performs the bitwise boolean operation SHIFT
        /// </summary>
        /// 
        /// <param name="count">
        /// The number of bits by which to shift.
        /// Positive count shift digits to the right, negative count shift them to the left.
        /// </param>
        public void Shift(int count)
        {
            if (count == 0) return;
            
            if (Abs(count) >= bitCount)
            {
                for (var i = 0; i < wordCount; i++) words[i] = 0;
                return;
            }

            var rightShift = count > 0;
            var shiftAmount = Abs(count);
            var wholeWordCount = shiftAmount / wordLength;
            shiftAmount %= wordLength; 
            
            if (shiftAmount != 0)
            {
                var overflowAmount = wordLength - shiftAmount;
                var carryBits = 0u;
                
                if (rightShift)
                {
                    for (var i = wordCount - 1; i >= wholeWordCount; i--)
                    {
                        var word = words[i];
                        words[i] = word >> shiftAmount | carryBits;
                        carryBits = word << overflowAmount;
                    }
                }
                else 
                {
                    for (var i = 0; i < wordCount - wholeWordCount; i++)
                    {
                        var word = words[i];
                        words[i] = word << shiftAmount | carryBits;
                        carryBits = word >> overflowAmount;
                    }
                }
            }
            
            if (rightShift)
            {
                for (var i = 0; i < wordCount; i++)
                {
                    if (i + wholeWordCount < wordCount) words[i] = words[i + wholeWordCount];
                    else words[i] = 0;
                }
            }
            else
            {
                for (var i = wordCount - 1; i >= 0; i--)
                {
                    if (i - wholeWordCount >= 0) words[i] = words[i - wholeWordCount];
                    else words[i] = 0;
                }
            }
        }

        #endregion

        // OPERATORS

        #region OPERATORS

        /// <summary>
        /// Bitwise AND
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>a &amp; b</returns>
        public static BitField192 operator &(BitField192 a, BitField192 b)
        {
            var c = a;
            c.And(in b);
            return c;
        }

        /// <summary>
        /// Bitwise AND on a single bit 
        /// </summary>
        /// <param name="a">Input bits</param>
        /// <param name="index">Bit index . Throws if index out of range [0:bitCount].</param>
        /// <returns>a &amp; single index mask</returns>
        public static BitField192 operator &(BitField192 a, int index)
        {
            var c = a;
            c.words[index / wordLength] &= 1u << index % wordLength;
            return c;
        }

        /// <summary>
        /// Bitwise OR
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>a | b</returns>
        public static BitField192 operator |(BitField192 a, BitField192 b)
        {
            var c = a;
            c.Or(in b);
            return c;
        }

        /// <summary> 
        /// Bitwise OR on a single bit
        /// Equivalent to SetBit(index)
        /// </summary>
        /// <param name="a">Input bits</param>
        /// <param name="index">Bit index. Throws if index out of range [0:bitCount].</param>
        /// <returns>a | single index mask</returns>
        public static BitField192 operator |(BitField192 a, int index)
        {
            var c = a;
            c.words[index / wordLength] |= 1u << index % wordLength;
            return c;
        }

        /// <summary>
        /// Bitwise XOR
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>a ^ b</returns>
        public static BitField192 operator ^(BitField192 a, BitField192 b)
        {
            var c = a;
            c.XOr(in b);
            return c;
        }

        /// <summary> 
        /// Bitwise XOR on a single bit
        /// Equivalent to ToggleBit(index)
        /// </summary>
        /// <param name="a">Input bits</param>
        /// <param name="index">Bit index. Throws if index out of range [0:bitCount].</param>
        /// <returns>a ^ single index mask</returns>
        public static BitField192 operator ^(BitField192 a, int index)
        {
            var c = a;
            c.words[index / wordLength] ^= 1u << index % wordLength;
            return c;
        }

        /// <summary>
        /// Bitwise NOT
        /// </summary>
        /// <param name="a">First operand</param>
        /// <returns>~a</returns>
        public static BitField192 operator ~(BitField192 a)
        {
            var c = a;
            c.Not();
            return c;
        }

        /// <summary>
        /// Bitwise LEFT-SHIFT
        /// Shifts the digits of a to the left, d times
        /// Negative arguments shift by -d to the right
        /// </summary>
        /// <param name="a">Input bits</param>
        /// <param name="d">Shift amount</param>
        /// <returns> a &lt;&lt; d</returns>
        public static BitField192 operator <<(BitField192 a, int d)
        {
            var c = a;
            c.Shift(-d);
            return c;
        }

        /// <summary>
        /// Bitwise RIGHT-SHIFT
        /// Shifts the digits of a to the right, d times
        /// Negative arguments shift by -d to the left
        /// </summary>
        /// <param name="a">Input bits</param>
        /// <param name="d">Shift amount</param>
        /// <returns> a &gt;&gt; d</returns>
        public static BitField192 operator >> (BitField192 a, int d)
        {
            var c = a;
            c.Shift(d);
            return c;
        }

        /// <summary>
        /// Determines if the bits of a and b are equal
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>a == b</returns>
        public static bool operator ==(BitField192 a, BitField192 b) => a.Equals(b);

        /// <summary>
        /// Determines if the bits of a and b are not equal
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>a != b</returns>
        public static bool operator !=(BitField192 a, BitField192 b) => !a.Equals(b);

        #endregion

        // INDEXER

        #region INDEXER

        /// <summary>
        /// Indexer into the bits for direct setting or querying.
        /// </summary>
        /// <param name="index">Index of the bit. Throws if not in the range [0:bitCount]</param>
        public bool this[int index]
        {
            get => GetBit(index);
            set
            {
                if (value) SetBit(index);
                else UnsetBit(index);
            }
        }

        #endregion

        // ENUMERATOR

        #region ENUMERATOR

        /// <summary>
        /// Enumerates the bits of the array from least-significant to most-significant.
        /// It is safe to change the array while enumerating.
        /// </summary>
        public ref struct Enumerator
        {
            /// <summary>
            /// Pointer to the bits
            /// </summary>
            private uint* _ptr;

            /// <summary>
            /// Index of the current bit
            /// </summary>
            private int _index;

            /// <summary>
            /// Create the enumerator with index at -1
            /// </summary>
            /// 
            /// <param name="ptr">
            /// Bits to enumerate
            /// </param>
            public Enumerator(uint* ptr)
            {
                _ptr = ptr;
                _index = -1;
            }

            /// <summary>
            /// Move to the next bit
            /// </summary>
            /// 
            /// <returns>
            /// If a bit is available via <see cref="Current"/>. If not, enumeration
            /// is done.
            /// </returns>
            public bool MoveNext()
            {
                _index++;
                if (_index > 0 && _index % wordLength == 0) _ptr++;
                return _index < bitCount;
            }

            /// <summary>
            /// Get the current bit. If <see cref="MoveNext"/> has not been called
            /// or the last call of <see cref="MoveNext"/> returned false, this
            /// function throws.
            /// </summary>
            /// 
            /// <value>
            /// The current bit
            /// </value>
            public bool Current
            {
                get
                {
                    IndexInBounds();
                    var mask = 1u << _index;
                    return (*_ptr & mask) == mask;
                }
            }

            /// <summary>
            /// Bounds check
            /// </summary>
            private void IndexInBounds()
            {
                if (_index < 0 && _index >= bitCount)
                    throw new IndexOutOfRangeException($"Index out of bounds: {_index}");
            }
        }

        /// <summary>
        /// Get an enumerator for the bits of this bit-field
        /// </summary>
        /// 
        /// <returns>
        /// An enumerator for  the bits of this bit-field
        /// </returns>
        public Enumerator GetEnumerator()
        {
            // Safe because Enumerator is a 'ref struct'
            fixed (uint* ptr = words) return new Enumerator(ptr);
        }

        #endregion

        // UTILITY

        #region UTILITY

        /// <summary>
        /// Get the positive absolute value of an integer
        /// </summary>
        private int Abs(int i)
        {
            if (i >= 0) return i;
            return -i;
        }

        /// <summary>
        /// Perform the modulo-remainder operation to decompose a full index index into a word-index and a bit-index.
        /// Convert the bit index into a mask.
        /// </summary>
        private static void GetMask(int index, out int wordIndex, out uint mask)
        {
            wordIndex = index / wordLength;
            mask = 1u << index % wordLength;
        }

        /// <summary>
        /// Bounds check for bit indices. Throws IndexOutOfRangeException when failed.
        /// </summary>
        private static void IndexInBounds(int index)
        {
            if (index < 0 || index >= bitCount) throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Get a string representation of the bit field
        /// </summary>
        /// 
        /// <returns>
        /// A newly-allocated string representing the bits of the array.
        /// </returns>
        public override string ToString()
        {
            const string typeName = nameof(BitField192);
            var chars = new char[typeName.Length + bitCount + wordCount + 4];
            var i = 0;
            for (; i < typeName.Length; ++i) chars[i] = typeName[i];
            chars[i++] = ' '; 
            chars[i++] = '('; 
            chars[i++] = ' ';
            for (var word = wordCount - 1; word >= 0; word--, ++i)
            {
                for (var digit = wordLength - 1; digit >= 0; digit--, ++i)
                {
                    var mask = 1u << digit;
                    chars[i] = (words[word] & mask) == mask ? '1' : '0';
                }
                chars[i] = ' ';
            }
            chars[i] = ')';
            return new string(chars);
        }

        #endregion

        // EQUATABLE

        #region EQUATABLE

        /// <summary>
        /// Check if this object equals another object
        /// </summary>
        /// 
        /// <param name="obj">
        /// Object to check. May be null.
        /// </param>
        /// 
        /// <returns>
        /// True if the given object is a BitArray32 and its bits are the same as this
        /// array's bits
        /// </returns>
        public override bool Equals(object obj) => obj is BitField192 other && Equals(other);

        /// <summary>
        /// Check if this BitField equals another BitField
        /// </summary>
        /// 
        /// <param name="other">
        /// Other BitField.
        /// </param>
        /// 
        /// <returns>
        /// If the given BitField's bits are the same as this BitField's bits
        /// </returns>
        public bool Equals(BitField192 other)
        {
            for (var i = 0; i < wordCount; i++)
                if (words[i] != other.words[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Get the hash code of this bitfield (by offsetting and hashing each word)
        /// </summary>
        public override int GetHashCode()
        {
            var hash = 17;
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            for (var i = 0; i < wordCount; i++) hash = 31 * hash + words[i].GetHashCode();
            return hash;
        }

        #endregion
    }
}