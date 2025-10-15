#ifndef RVSIM_INSMEM_H
#define RVSIM_INSMEM_H

#include <stdint.h>

#define MEMSIZE (1024 * 16)

class InsMem {
private: 
    uint8_t _memory[MEMSIZE];
    int x;
public:
    InsMem();
    // ~InsMem(); Destruktor wird aufgerufen, wenn Objekt zerstört wird
    uint32_t fetchInstruction(uint32_t addr);
};

#endif // RVSIM_INSMEM_H
