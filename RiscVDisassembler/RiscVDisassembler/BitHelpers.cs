namespace RiscVDisassembler
{
    internal static class BitHelpers
    {
        public static int SignExtend12(int imm)
        {
            return (imm & 0x800) != 0 ? imm | unchecked((int)0xFFFFF000) : imm;
        }

        public static int SignExtend21(int imm)
        {
            return (imm & 0x100000) != 0 ? imm | unchecked((int)0xFFE00000) : imm;
        }
    }
}