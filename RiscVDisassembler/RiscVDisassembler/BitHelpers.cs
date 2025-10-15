namespace RiscVDisassembler
{
    /// <summary>
    /// Utility methods for bit manipulation and sign extension.
    /// </summary>
    internal static class BitHelpers
    {
        /// <summary>
        /// Sign-extends a 12-bit immediate value to 32 bits.
        /// </summary>
        public static int SignExtend12(int imm)
        {
            return (imm & 0x800) != 0 ? imm | unchecked((int)0xFFFFF000) : imm;
        }

        /// <summary>
        /// Sign-extends a 21-bit immediate value to 32 bits.
        /// </summary>
        public static int SignExtend21(int imm)
        {
            return (imm & 0x100000) != 0 ? imm | unchecked((int)0xFFE00000) : imm;
        }
    }
}