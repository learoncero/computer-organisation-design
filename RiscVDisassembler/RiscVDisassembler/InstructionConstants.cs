namespace RiscVDisassembler
{
    internal static class InstructionConstants
    {
        // Bit masks
        public const uint OpcodeMask = 0x7F;      // bits 6:0
        public const uint RdMask = 0x1F;          // 5 bits
        public const uint Funct3Mask = 0x7;       // 3 bits
        public const uint Rs1Mask = 0x1F;         // 5 bits
        public const uint Rs2Mask = 0x1F;         // 5 bits
        public const uint Funct7Mask = 0x7F;      // 7 bits
        public const uint Imm12Mask = 0xFFF;      // 12 bits
        public const uint Imm20Mask = 0xFFFFF000; // upper 20 bits
        public const uint ShiftMask = 0x1F;       // 5 bits for shift amount

        // Bit shift positions
        public const int OpcodeShift = 0;
        public const int RdShift = 7;
        public const int Funct3Shift = 12;
        public const int Rs1Shift = 15;
        public const int Rs2Shift = 20;
        public const int Funct7Shift = 25;
        public const int Imm12Shift = 20;

        // Opcodes
        public const uint OpcodeRType = 0x33;
        public const uint OpcodeITypeArith = 0x13;
        public const uint OpcodeITypeLoad = 0x03;
        public const uint OpcodeSType = 0x23;
        public const uint OpcodeJalr = 0x67;
        public const uint OpcodeJal = 0x6F;
        public const uint OpcodeLui = 0x37;
        public const uint OpcodeAuipc = 0x17;
        public const uint OpcodeBType = 0x63;
    }
}