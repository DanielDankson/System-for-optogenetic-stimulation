// include libraries
#include <Wire.h>
#include <BH1750.h>
#include <i2cdetect.h>
#include <Servo.h>
#include <DHT.h>

// define 
#define DHTPIN 7
#define DHTTYPE DHT11

// initialize objects
BH1750 lightmeter(0x23); // BH1750 object
DHT dht = DHT(DHTPIN,DHTTYPE); // DHT object
Servo Mservo; // Servo object

// BH1750 Use port A5 and A4
// DHT11 USE port 7 
// Servo Use Port 9

void setup() {
  // Initialize Components
  Serial.begin(9600); // initialize Serial
  Wire.begin();       // initialize Wire
  lightmeter.begin(); // initialize GY-302 module
  Mservo.attach(9);   // initialize Servo
  dht.begin();        // initialize DHT11 module
  
 // Initialize servo to it's zero position
 for(int posp = Mservo.read(); posp <= 102; posp++)
       {
        if(posp < 102){
          Mservo.write(posp);
        }
        delay(15);   
      }        
}

void loop() {
  // reading lux the light level
  float lux = lightmeter.readLightLevel();
  // reading humidity in %
  float h = dht.readHumidity();
  // reading temperature as celsius, put true inside () to read fahrenheit
  float t = dht.readTemperature();
  //bool to decide if the sequence have started or not
  bool start = false;
  Serial.flush(); 
  //serial information (this is how the message will appear in c#)
  //(startbit,bildebit,temperature,humidity,lux,)
          
//start 
 if(lux > 400){
      for(int i = 0; i < 1; i){
         float lux = lightmeter.readLightLevel();
         float h = dht.readHumidity();
         float t = dht.readTemperature();
         Serial.print("1");
         Serial.print(",");
         Serial.print("0");
         Serial.print(",");
         Serial.print(t);
         Serial.print(",");
         Serial.print(h);
         Serial.print(",");
         Serial.print(lux);
         Serial.println(",");            
         delay(1000);
         
         if(lux < 400){
          i++;
        }       
      }
      start = true;
    }
//if the sequence have not started the code will still give the serial information
if(start == false){
         float lux = lightmeter.readLightLevel();
         float h = dht.readHumidity();
         float t = dht.readTemperature();
         Serial.print("0");
         Serial.print(",");
         Serial.print("0");
         Serial.print(",");
         Serial.print(t);
         Serial.print(",");
         Serial.print(h);
         Serial.print(",");
         Serial.print(lux);
         Serial.println(",");         
         delay(1000);
}
//if the sequence have started, the servo will move up and down taking information on the way. 
if(start == true)
{
  for(int posn = Mservo.read(); posn >= 25; posn--) // move servo up
      {
        if(posn > 25){
           Mservo.write(posn);      
        }
       delay(15);       
      }     
      // this doesnt need a for function, we only included it incase they wanted to repeat the prosess
      for(int i = 0; i < 1; i++){
         float lux = lightmeter.readLightLevel();
         float h = dht.readHumidity();
         float t = dht.readTemperature();
         Serial.print("1");
         Serial.print(",");
         Serial.print("0");
         Serial.print(",");
         Serial.print(t);
         Serial.print(",");
         Serial.print(h);
         Serial.print(",");
         Serial.print(lux);
         Serial.println(",");         
         delay(1000);
         lux = lightmeter.readLightLevel();
         h = dht.readHumidity();
         t = dht.readTemperature();
         Serial.print("1");
         Serial.print(",");
         Serial.print("0");
         Serial.print(",");
         Serial.print(t);
         Serial.print(",");
         Serial.print(h);
         Serial.print(",");
         Serial.print(lux);
         Serial.println(",");         
         delay(1000);
         lux = lightmeter.readLightLevel();
         h = dht.readHumidity();
         t = dht.readTemperature();
         Serial.print("1");
         Serial.print(",");
         Serial.print("1"); // taking picture
         Serial.print(",");
         Serial.print(t);
         Serial.print(",");
         Serial.print(h);
         Serial.print(",");
         Serial.print(lux);
         Serial.println(",");         
         delay(1000);
         lux = lightmeter.readLightLevel();
         h = dht.readHumidity();
         t = dht.readTemperature();
         Serial.print("1");
         Serial.print(",");
         Serial.print("0");
         Serial.print(",");
         Serial.print(t);
         Serial.print(",");
         Serial.print(h);
         Serial.print(",");
         Serial.print(lux);
         Serial.println(",");         
         delay(1000);
      }  
      for(int posp = Mservo.read(); posp <= 102; posp++) // move servo down
       {
        if(posp < 102){
          Mservo.write(posp);
        }
        delay(15);   
      }
      for(int s = 0; s <= 3; s++){
         float lux = lightmeter.readLightLevel();
         float h = dht.readHumidity();
         float t = dht.readTemperature();
         Serial.print("1"); 
         Serial.print(",");
         Serial.print("0");
         Serial.print(",");
         Serial.print(t);
         Serial.print(",");
         Serial.print(h);
         Serial.print(",");
         Serial.print(lux);
         Serial.println(",");         
         delay(1000); 
      }        
         start = false;
  }   
  delay(15);              
}                      
 
 

    
  
 
