#include <SoftwareSerial.h>

// Attach the serial display's RX line to digital pin 2
SoftwareSerial LCD(19,18); // pin 18 = TX, pin 19 = RX (unused)

const int dd = 54;			    // number of digital ports
const int aa = 16;			     // number of analog ports
int D[dd];					// define digital enables
int R[dd];					// define digital directions
int T[dd];					// define digital triggers
int V[dd];					// define digital values
int A[aa];					// define analog enables
int S[aa];					// define analog triggers
int W[aa];					// define analog values
int cp = 0;				    // char pointer
int x = 0;
int y = 0;
boolean cfound = false;			  // complete command found
byte stringIn[200];			   // commandbuffer
long previousMillis = 0;		    // intervalcheck for watchdog
long interval = 1000;			 // interval between watchdogs
char cr = 13;

#define SOC 40
#define EOC 41

void setup() {
  for (int i=0; i < dd; i++) {
    D[i]=0;				    // enable digital port (default to 0 - disabled)
    R[i]=1;				    // direction of digital port (2=PWM, 1=output, 0=input) (default to 1)
    T[i]=0;				    // enable digital trigger (default to 0 - no trigger)
    V[i]=-1;				    // value of digital port (default to -1 for first trigger)
  }
  for (int i=0; i < aa; i++) {
    A[i]=0;				    // enable analog port (default to 0 - disabled)
    S[i]=0;				    // enable analog trigger (default to 0 - no trigger)
    W[i]=-1;				    // value of analog port (default to -1 for first trigger)
  }
  
  pinMode(20, INPUT);
  pinMode(21, INPUT);

  Serial.begin(9600);
  LCD.begin(9600); // set up serial port for 9600 baud
  delay(500); // wait for display to boot up
  Serial.print("(H)");
}

void loop() {
  int sa;
  byte bt;
  sa = Serial.available();		  // count serial buffer bytes
  if (sa > 0) {				  // if buffer not empty process buffer content
    for (int i=0; i < sa; i++){
	bt = Serial.read();		// read one byte from the serial buffer
	stringIn[cp] = bt;
	cp++;
	if (bt == EOC) {			  // check for last command char )
	  cfound = true;
	  break;	   			  // end for loop
	}
    }
  }

  if (cfound) {
    if (int(stringIn[0]) == SOC) {	//check if first char of command is (
	exCommand();
    }
    cleanstring();
    cfound = false;
  }

  for (int i=0; i<dd; i++) {	     // check all digital ports
    if ((D[i] == 1) && (R[i] == 0)) {  // if port enabled and set to input, update
	int dr = digitalRead(i);
	if (dr != V[i]) {		    // value changed?
	  V[i] = dr;			   // store value
	  if (T[i] == 1) {		   // trigger enabled?
	    Serial.print("(D");
	    Serial.print(byte(i));
	    Serial.print(byte(V[i]));
	    Serial.print(")");
	  }
	}
    }
  }
//  for (int i=0; i<aa; i++) {		// check analog values
//    if (A[i] == 1) {
//	int ar = analogRead(i);
//	if (ar != W[i]) {		     // value changed?
//	  if (S[i] > 0) {		     // analog trigger set?
//	    if (((ar - W[i]) > S[i]) || ((W[i] - ar) > S[i])) {
//		Serial.print("(A");
//		Serial.print(byte(i));
//		Serial.print(highByte(ar));
//		Serial.print(lowByte(ar));
//		Serial.print(")");
//	    }
//	  }
//	  W[i] = ar;			    // store value
//	}
//    }
//  }

  if (millis() - previousMillis > interval) {
    previousMillis = millis();
    Serial.print("(H)");		    // send watchdog trigger
  }

}

void cleanstring(void) {
  for (int i=0; i<=200; i++) {
    stringIn[i] = 0;			// null out string
    cp = 0;
  }
}

void exCommand(void) {
  char c = stringIn[1];		    // command type
  int n = int(stringIn[2]);		// portnumber
  int v = int(stringIn[3]);		// value
  
  if (n > dd) {
      n = 41;                      //Special case working with Port 41 (also code for EOC)
  }

  switch (c) {
  case 'D':				// enable digital pin
    if (v == 0) {
	D[n] = 0;
    }
    else {
	D[n] = 1;
    }
    break;
  case 'A':				// enable analog pin
    if (v == 1) {
	A[n] = 1;
    }
    else {
	A[n] = 0;
    }
    break;
  case 'R':				// set direction of digital pin
    switch (v) {
    case 0:				  // input
	R[n] = 0;
	pinMode(n, INPUT);
        break;
    case 1:				// output digital
	R[n] = 1;
	pinMode(n, OUTPUT);
	break;
    case 2:				// output analog
	R[n] = 2;
	pinMode(n, OUTPUT);
	break;
    }
    break;
  case 'T':				// set trigger on digital pin
    if (v == 0) {
	T[n] = 0;
    }
    else {
	T[n] = 1;
    }
    break;
  case 'S':				// set trigger on analog pin
    S[n] = v;
    break;
  case 'V':				// get value of digital pin
    V[n] = digitalRead(n);
    Serial.print("(D");
    Serial.print(byte(n));
    Serial.print(byte(V[n]));
    Serial.print(")");
    break;
  case 'W':				// get value of analog pin
    W[n] = analogRead(n);
    Serial.print("(A");
    Serial.print(byte(n));
    Serial.print(highByte(W[n]));
    Serial.print(lowByte(W[n]));
    Serial.print(")");
    break;
  case 'P':				    // write value to digital port
    switch ( R[n]) {
	case 1:				  // digital output
	  digitalWrite(n, v);
	  break;
	case 2:				  // PWM output
	  analogWrite(n, v);
	  break;
    }
	break;
  case '1':                               // Update First Row of LCD message display
    LCD.write(254); // move cursor to beginning of first line
    LCD.write(128);
    x = 2;
    while (stringIn[x] != EOC)
       {
       LCD.write(stringIn[x]);
       x++;
       }
    break;
  case '2':                               // Update Second Row of LCD message display
    LCD.write(254); // move cursor to beginning of first line
    LCD.write(192);
    y = 2;
    while (stringIn[y] != EOC)
       {
       LCD.write(stringIn[y]);
       y++;
       }
    break;
  }

}

