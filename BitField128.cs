using System;

/// <summary>
/// Unmanaged struct used to represent large collections of flags.
/// Use this when the number of unique flags exceeds the capacity of a single integer.
/// Words are represented as a fixed-size, unmanaged array of 32-bit unsigned integers.
/// All standard bitwise operators are implemented ( & | ^ ~ << >> )
/// </summary>
public unsafe struct BitField128 : IEquatable<BitField128>
{
    // PROPERTIES AND FIELDS
    
    #region DATA
    
    /// <summary>
    /// Unmanaged array of uint32 words that make up the bitfield
    /// </summary>
    private fixed uint words[wordCount];
    
    /// <summary>
    /// Number of words that make up the bit field
    /// </summary>
    public const int wordCount = 4;
    
    /// <summary>
    /// Number of bits in each word 
    /// </summary>
    public const int wordLength = 32;
    
    /// <summary>
    /// Total number of bits stored in this bitfield
    /// </summary>
    public const int bitCount = 128;
    
    #endregion
    
    // PROPERTIES
    
    #region PROPERTIES
    
    /// <summary>
    /// The default bitfield (all zeros)
    /// </summary>
    public static BitField128 none => new BitField128();
    
    /// <summary>
    /// A new bitfield with all bits set to 1
    /// </summary>
    public static BitField128 all => ~new BitField128();
    
    #endregion

    // BIT MANIPULATION

    #region BIT_MANIPULATION
    
    /// <summary>
    /// Set a single bit to 1
    /// </summary>
    /// 
    /// <param name="index">
    /// Index of the bit to set. Throws if out of range [0:127].
    /// </param>
    public void SetBit(int index)
    {
        IndexInBounds(index);
        GetMask(index, out var wordIndex, out var mask);
        words[wordIndex] |= mask;
    }
    
    /// <summary>
    /// Set a single bit to 0
    /// </summary>
    /// 
    /// <param name="index">
    /// Index of the bit to set. Throws if out of range [0:127].
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
    /// Index of the bit to flip. Throws if out of range [0:127].
    /// </param>
    public void FlipBit(int index)
    {
        IndexInBounds(index);
        GetMask(index, out var wordIndex, out var mask);
        words[wordIndex] ^= mask;
    }

    /// <summary>
    /// Set all the bits that match a mask to 1 ( bits |= mask )
    /// </summary>
    /// 
    /// <param name="mask">
    /// Mask of bits to set
    /// </param>
    public void SetBits(in BitField128 mask) => Or(in mask);
    
    /// <summary>
    /// Set all the bits that match a mask to 0 ( bits &= ~mask )
    /// </summary>
    /// 
    /// <param name="mask">
    /// Mask of bits to unset
    /// </param>
    public void UnsetBits(in BitField128 mask) => AndNot(in mask);

    
    /// <summary>
    /// Flips all the bits that match a mask ( bits ^= mask )
    /// </summary>
    /// 
    /// <param name="mask">
    /// Mask of bits to flip
    /// </param>
    public void FlipBits(in BitField128 mask) => XOr(in mask);
    
    #endregion
    
    // QUERIES
    
    #region QUERIES

    /// <summary>
    /// Get the state of a single bit
    /// </summary>
    /// 
    /// <param name="index">
    /// Index of the bit
    /// </param>
    public bool GetBit(int index)
    {
        IndexInBounds(index);
        GetMask(index, out var wordIndex, out var mask);
        return (words[wordIndex] & mask) == mask;
    }

    /// <summary>
    /// Determine if all bits are set to 0
    /// </summary>
    public bool IsEmpty()
    {
        for (var i = 0; i < wordCount; i++)
            if (words[i] != 0)
                return false;

        return true;
    }

    /// <summary>
    /// Determine if ALL of the bits from a given mask are set 
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    public bool HasAllOf(in BitField128 mask) => (this & mask) == mask;

    /// <summary>
    /// Determine if ANY of the bits from a given mask are set 
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    public bool HasAnyOf(in BitField128 mask) => !(this & mask).IsEmpty();
    
    /// <summary>
    /// Determine if NONE of the bits from a given mask are set 
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    public bool HasNoneOf(in BitField128 mask) => (this & mask).IsEmpty();

    #endregion
    
    // BOOLEAN OPERATIONS

    #region BOOLEAN_OPERATIONS
    
    // And, Not, Or and XOr are the same operations performed element-by-element on the underlying uints

    /// <summary>
    /// Perform the bitwise boolean operation AND (bits & mask)
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    private void And(in BitField128 mask)
    {
        for (var i = 0; i < wordCount; i++) 
            words[i] &= mask.words[i];
    }

    /// <summary>
    /// Perform the bitwise boolean operation AND-NOT (bits & ~mask)
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    private void AndNot(in BitField128 mask)
    {
        for (var i = 0; i < wordCount; i++)
            words[i] &= ~mask.words[i];
    }

    /// <summary>
    /// Perform the bitwise boolean operation NAND ~(bits & mask)
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    private void Nand(in BitField128 mask)
    {
        for (var i = 0; i < wordCount; i++)
            words[i] = ~(words[i] & mask.words[i]);
    }

    /// <summary>
    /// Perform the bitwise boolean operation OR (bits | mask)
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    private void Or(in BitField128 mask)
    {
        for (var i = 0; i < wordCount; i++) 
            words[i] |= mask.words[i];
    }

    /// <summary>
    /// Perform the bitwise boolean operation OR-NOT (bits | ~mask)
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    private void OrNot(in BitField128 mask)
    {
        for (var i = 0; i < wordCount; i++) 
            words[i] |= ~mask.words[i];
    }

    /// <summary>
    /// Perform the bitwise boolean operation NOR ~(bits | mask)
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    private void Nor(in BitField128 mask)
    {
        for (var i = 0; i < wordCount; i++) 
            words[i] = ~(words[i] | mask.words[i]);
    }


    /// <summary>
    /// Perform the bitwise boolean operation XOR (bits ^ mask)
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    private void XOr(in BitField128 mask)
    {
        for (var i = 0; i < wordCount; i++) 
            words[i] ^= mask.words[i];
    }

    /// <summary>
    /// Perform the bitwise boolean operation XOR-NOT (bits ^ ~mask)
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    private void XOrNot(in BitField128 mask)
    {
        for (var i = 0; i < wordCount; i++) 
            words[i] ^= ~mask.words[i];
    }
    
    /// <summary>
    /// Perform the bitwise boolean operation NOT-XOR ~(bits ^ ~mask)
    /// </summary>
    /// 
    /// <param name="mask">
    /// Query mask
    /// </param>
    private void NotXOr(in BitField128 mask)
    {
        for (var i = 0; i < wordCount; i++) 
            words[i] = ~(words[i] ^ mask.words[i]);
    }
    
    /// <summary>
    /// Perform the bitwise boolean operation NOT (~bits)
    /// </summary>
    private void Not()
    {
        for (var i = 0; i < wordCount; i++) 
            words[i] = ~words[i];
    }

    /// <summary>
    /// Perform the bitwise boolean operation SHIFT
    /// </summary>
    /// 
    /// <param name="count">
    /// The number of bits by which to shift.
    /// Positive count shift digits to the right, negative count shift them to the left.
    /// </param>
    private void Shift(int count)
    {
        // The shift operation is more complicated because it pushes bits from one word into another
        // we must "carry" the bits that are shifted out of one word into the adjacent word
        // for shift amounts that are greater than the word length, we can decompose the operation
        // into two parts, shifting by an amount smaller than the word length and then copying words 
        // in whole steps from one array element to another.
        // Right and left shift require traversing the underlying array in the opposite orders.
        
        // if shifting by 0 do nothing
        if (count == 0) return;
         
        // if shifting by more than the total number of bits stored,
        // set every bit to zero
        if (Abs(count) >= bitCount)
        {
            for (var i = 0; i < wordCount; i++) words[i] = 0;
            return;
        }
        
        // This operation can be split into two simpler operations:
        // (1) shift the bits by an amount smaller than the word length (using bitwise math)
        // (2) shift the result by a whole number of words (by shifting the positions of words in the array)
        
        // find the the quotient and remainder of the shift amount over the word length
        // the quotient is the number of whole words to shift
        var wholeWordCount = Abs(count / wordLength);
        // the remainder is the number of bits to shift within each word
        var shiftAmount = Abs(count % wordLength);
        // get the direction of the shift
        var rightShift = count > 0;

        // (1): shift by the remainder after division by word length
        if (shiftAmount != 0)
        {
            // find the carry bits that overflow from one word to the next
            // this is the same as shifting the word in the opposite direction by wordLength - shiftAmount
            var overflowAmount = wordLength - shiftAmount;
            // the first word will be filled in with zeros from the trailing side, so the the carry bits are zero
            var carryBits = 0u;
            
            // no need to process elements that are with wordCount of the opposite edge of the array
            // these will be pushed out of the bitfield
            if (rightShift) // RIGHT-SHIFT
            {
                // work backwards through the array pushing bits right
                for (var i = wordCount - 1; i >= wholeWordCount; i--)
                {
                    // get the current word
                    var word = words[i];
                    // the first term is the [current word] shifted in the target direction by the [shift amount]
                    // the second term is the bits carried over from the previous word (0 if this is the first word)
                    // [carry bits] are the previous word's bits shifted in the opposite direction by the [overflow amount]
                    // [term1] OR [term2] gives the resulting word 
                    words[i] = word >> shiftAmount | carryBits;
                    // store the [carry bits] for the next word 
                    carryBits = word << overflowAmount;
                }
            }
            else // LEFT-SHIFT
            {
                // work forwards through the array pushing bits left
                for (var i = 0; i < wordCount - wholeWordCount; i++)
                {
                    // get the current word
                    var word = words[i];
                    // the first term is the [current word] shifted in the target direction by the [shift amount]
                    // the second term is the bits carried over from the previous word (0 if this is the first word)
                    // [carry bits] are the previous word's bits shifted in the opposite direction by the [overflow amount]
                    // [term1] OR [term2] gives the resulting word 
                    words[i] = word << shiftAmount | carryBits;
                    // store the carry bits for the next word
                    carryBits = word >> overflowAmount;
                }
            }
        }

        // (2): shift whole words by the quotient of the division
        if (rightShift)
        {
            // work forwards through the array
            for (var i = 0; i < wordCount; i++)
            {
                // if possible, replace the entry at the current index with the entry at (index + wholeWordCount)
                if (i + wholeWordCount < wordCount) words[i] = words[i + wholeWordCount];
                // otherwise fill in with zeros
                else words[i] = 0;
            }
        }
        else
        {
            // work backwards through the array
            for (var i = wordCount-1; i >= 0; i--)
            {
                // if possible, replace the entry at the current index with the entry at (index - wholeWordCount)
                if (i - wholeWordCount >= 0) words[i] = words[i - wholeWordCount];
                // otherwise fill in with zeros
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
    /// <returns>a & b</returns>
    public static BitField128 operator &(BitField128 a, BitField128 b)
    {
        var c = a;
        c.And(in b);
        return c;
    } 
    
    /// <summary>
    /// Bitwise AND on a single bit 
    /// </summary>
    /// <param name="a">Input bits</param>
    /// <param name="b">Bit index</param>
    /// <returns></returns>
    public static BitField128 operator &(BitField128 a, int index)
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
    public static BitField128 operator |(BitField128 a, BitField128 b)
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
    /// <param name="index">Bit index</param>
    /// <returns>a |= single index mask</returns>
    public static BitField128 operator |(BitField128 a, int index)
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
    public static BitField128 operator ^(BitField128 a, BitField128 b)
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
    /// <param name="index">Bit index</param>
    /// <returns>a ^= single index mask</returns>
    public static BitField128 operator ^(BitField128 a, int index)
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
    public static BitField128 operator ~(BitField128 a)
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
    /// <returns> a left-shifted by d </returns>
    public static BitField128 operator <<(BitField128 a, int d)
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
    /// <returns> a right-shifted by d </returns>
    public static BitField128 operator >>(BitField128 a, int d)
    {
        var c = a;
        c.Shift(d);
        return c;
    }
    
    /// <summary>
    /// Check if the bits of a and b are equal
    /// </summary>
    /// <param name="a">First operand</param>
    /// <param name="b">Second operand</param>
    /// <returns>a == b</returns>
    public static bool operator ==(BitField128 a, BitField128 b) => a.Equals(b);
    
    /// <summary>
    /// Check if the bits of a and b are not equal
    /// </summary>
    /// <param name="a">First operand</param>
    /// <param name="b">Second operand</param>
    /// <returns>a != b</returns>
    public static bool operator !=(BitField128 a, BitField128 b) => !a.Equals(b);
    
    #endregion

    // INDEXER
    
    #region INDEXER
    
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
    /// Enumerates the bits of the array from least-significant to
    /// most-significant. It is safe to change the array while enumerating.
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
            if (_index <= wordLength) return _index * wordLength < bitCount;
            _index %= wordLength;
            _ptr++;
            return _index * wordLength < bitCount;
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
        public void IndexInBounds()
        {
            if (_index >= 0 && _index < bitCount) 
                throw new IndexOutOfRangeException($"Index out of bounds: {_index}");
        }
    }
    
    /// <summary>
    /// Get an enumerator for this array's bits
    /// </summary>
    /// 
    /// <returns>
    /// An enumerator for this array's bits
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
    /// Perform the modulo-remainder operation to decompose a bit index into a word index and a bit index in that word.
    /// Convert the bit index into a mask.
    /// </summary>
    private void GetMask(int index, out int wordIndex, out uint mask)
    {
        wordIndex = index / wordLength;
        mask = 1u << index % wordLength;
    }
    
    /// <summary>
    /// Bounds check for bit indices. Throws IndexOutOfRangeException when failed.
    /// </summary>
    private void IndexInBounds(int index)
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
        const string header = "BitField ( ";
        const int headerLen = 11; 
        var chars = new char[headerLen + bitCount + wordCount + 2];
        var i = 0;
        for (; i < headerLen; ++i) chars[i] = header[i];
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
    public override bool Equals(object obj) => obj is BitField128 other && Equals(other);
    
    /// <summary>
    /// Check if this BitField equals another BitField
    /// </summary>
    /// 
    /// <param name="arr">
    /// Array to check
    /// </param>
    /// 
    /// <returns>
    /// If the given BitField's bits are the same as this BitField's bits
    /// </returns>
    public bool Equals(BitField128 other)
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
        for(var i = 0; i < wordCount; i++) hash = 31 * hash + words[i].GetHashCode();
        return hash;
    }
    
    #endregion
}