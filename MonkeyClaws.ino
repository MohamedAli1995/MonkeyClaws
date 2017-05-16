#include "jungle.h"
Jungle glove;  
void dmpDataReady() {
 mpuInterrupt = true;
}
void setup() {
     

  glove.init();
   attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);
}



// ================================================================
// ===                    MAIN PROGRAM LOOP                     ===
// ================================================================

void loop() {

  glove.main();
}
