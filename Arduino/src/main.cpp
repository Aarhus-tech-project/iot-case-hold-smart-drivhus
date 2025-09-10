#include <Wire.h>
#include <SPI.h>
#include <Adafruit_BME280.h>
#include <Adafruit_Sensor.h>
#include "Adafruit_TSL2591.h"
#include "Adafruit_CCS811.h"
#include <WiFi.h>
#include <PubSubClient.h>
#include <HTTPClient.h>
#include <WiFiClientSecure.h>

/*
#define BME_SCK 18
#define BME_MISO 19
#define BME_MOSI 23
#define BME_CS 5 */

#define ledPin 2

//jordfugtighed 34 dev module

  #define sensorPin 34

  //#define SEALEVELPRESSURE_HPA (1013.25)

  const char* WIFI_SSID     = "h4prog";
  const char* WIFI_PASS     = "1234567890";

  //vandsensor 35 dev module

  #define analogPin = 35

  int analogpin;

/*
int led = 13;
int val = 0; 
int data = 0; */

//const char* API_ROUTE = "https://webappnamename-dkfxb8g3eubvhabr.swedencentral-01.azurewebsites.net/api/sensor";
const char* url = "https://webappnamename-dkfxb8g3eubvhabr.swedencentral-01.azurewebsites.net/api/sensor";


// Azure IoT Hub
const char* HUB    = "GreenHouseIotHub.azure-devices.net";
const char* DEVICE = "WifiArduino";
const char* USER   = "GreenHouseIotHub.azure-devices.net/WifiArduino/?api-version=2021-04-12";

const char* PASS   = "SharedAccessSignature sr=GreenHouseIotHub.azure-devices.net%2Fdevices%2FWifiArduino&sig=TunkufJg0miIZ61RnV8JpnMYFxTJqkg5KIOZ9mp9O7M%3D&se=1788514946";

unsigned long delayTime; 
int lux;
int temp;
int tryk;
int fugtighed;
int co2;
int vandmaengde;
int jordfugtighed;

Adafruit_TSL2591 tsl = Adafruit_TSL2591(2591);
Adafruit_BME280 bme; 
Adafruit_CCS811 ccs;

WiFiClientSecure net;
PubSubClient mqtt(net);

void onMessage(char* topic, byte* payload, unsigned int len) 
{
  Serial.print("Message: ");
  while (len--) Serial.print((char)*payload++);
  Serial.println();
}

void postOnce() {
  if (WiFi.status() != WL_CONNECTED) { Serial.println("WiFi not connected"); return; }

  WiFiClientSecure client;
  client.setInsecure();                 // ✅ QUICK & DIRTY: skip cert validation

  HTTPClient http;
  if (!http.begin(client, url)) { Serial.println("http.begin failed"); return; }
  http.addHeader("Content-Type", "application/json");


   String payload = "{";
  payload += "\"lux\":" + String(lux) + ",";
  payload += "\"soilhumidity\":" + String(jordfugtighed) + ",";
  payload += "\"temp\":" + String(temp) + ",";
  payload += "\"pressure\":" + String(tryk) + ",";
  payload += "\"humidity\":" + String(fugtighed) + ",";
  payload += "\"co2\":" + String(co2) + ",";
  payload += "\"waterlevel\":" + String(vandmaengde);
  payload += "}";


  // Match your API: { "name": "..." }
  int code = http.POST(payload);
  Serial.printf("POST code: %d\n", code);
  if (code > 0) Serial.println(http.getString());
  http.end();


}


void connectWiFi()
{
    Serial.println("Connecting to WiFi...");
  WiFi.begin(WIFI_SSID, WIFI_PASS);

  // Wait until connected
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

   Serial.println("");
  Serial.println("WiFi connected!");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
} 


void connectAzure()
{
  net.setInsecure();

      mqtt.setServer(HUB, 8883);
  mqtt.setCallback(onMessage);
    while (!mqtt.connected())
    mqtt.connect(DEVICE, USER, PASS);

  mqtt.subscribe("devices/WifiArduino/messages/devicebound/#");
  Serial.println("Connected to Azure IoT Hub!");
}

void displaySensorDetails(void)
{
  sensor_t sensor;
  tsl.getSensor(&sensor);
  Serial.println(F("------------------------------------"));
  Serial.print  (F("Sensor:       ")); Serial.println(sensor.name);
  Serial.print  (F("Driver Ver:   ")); Serial.println(sensor.version);
  Serial.print  (F("Unique ID:    ")); Serial.println(sensor.sensor_id);
  Serial.print  (F("Max Value:    ")); Serial.print(sensor.max_value); Serial.println(F(" lux"));
  Serial.print  (F("Min Value:    ")); Serial.print(sensor.min_value); Serial.println(F(" lux"));
  Serial.print  (F("Resolution:   ")); Serial.print(sensor.resolution, 4); Serial.println(F(" lux"));  
  Serial.println(F("------------------------------------"));
  Serial.println(F(""));
  delay(500);
}

void startCSS()
{
  Serial.println("CCS811 test");

  if(!ccs.begin()){
    Serial.println("Failed to start sensor! Please check your wiring.");
    while(1);
  }

  while(!ccs.available());
}

void startBME()
{
  unsigned status;
  status = bme.begin();  
    if (!status) {
        Serial.println("Could not find a valid BME280 sensor, check wiring, address, sensor ID!");
        Serial.print("SensorID was: 0x"); Serial.println(bme.sensorID(),16);
        Serial.print("        ID of 0xFF probably means a bad address, a BMP 180 or BMP 085\n");
        Serial.print("   ID of 0x56-0x58 represents a BMP 280,\n");
        Serial.print("        ID of 0x60 represents a BME 280.\n");
        Serial.print("        ID of 0x61 represents a BME 680.\n");
        while (1) delay(10);
    }
    
    Serial.println("-- Default Test --");
    delayTime = 1000;

    Serial.println();
}

void configureSensor(void)
{
  // You can change the gain on the fly, to adapt to brighter/dimmer light situations
  //tsl.setGain(TSL2591_GAIN_LOW);    // 1x gain (bright light)
  tsl.setGain(TSL2591_GAIN_MED);      // 25x gain
  //tsl.setGain(TSL2591_GAIN_HIGH);   // 428x gain
  
  // Changing the integration time gives you a longer time over which to sense light
  // longer timelines are slower, but are good in very low light situtations!
  // tsl.setTiming(TSL2591_INTEGRATIONTIME_100MS);  // shortest integration time (bright light)
  // tsl.setTiming(TSL2591_INTEGRATIONTIME_200MS);
  tsl.setTiming(TSL2591_INTEGRATIONTIME_300MS);
  // tsl.setTiming(TSL2591_INTEGRATIONTIME_400MS);
  // tsl.setTiming(TSL2591_INTEGRATIONTIME_500MS);
  // tsl.setTiming(TSL2591_INTEGRATIONTIME_600MS);  // longest integration time (dim light)

  Serial.println(F("------------------------------------"));
  Serial.print  (F("Gain:         "));
  tsl2591Gain_t gain = tsl.getGain();
  switch(gain)
  {
    case TSL2591_GAIN_LOW:
      Serial.println(F("1x (Low)"));
      break;
    case TSL2591_GAIN_MED:
      Serial.println(F("25x (Medium)"));
      break;
    case TSL2591_GAIN_HIGH:
      Serial.println(F("428x (High)"));
      break;
    case TSL2591_GAIN_MAX:
      Serial.println(F("9876x (Max)"));
      break;
  }
  Serial.print  (F("Timing:       "));
  Serial.print((tsl.getTiming() + 1) * 100, DEC); 
  Serial.println(F(" ms"));
  Serial.println(F("------------------------------------"));
  Serial.println(F(""));
}

void startTSL2591()
{

   Serial.println(F("Starting Adafruit TSL2591 Test!"));
  
  if (tsl.begin()) 
  {
    Serial.println(F("Found a TSL2591 sensor"));
  } 
  else 
  {
    Serial.println(F("No sensor found ... check your wiring?"));
    while (1);
  }
    
  //displaySensorDetails();
  configureSensor();
}

void simpleRead(void)
{
  uint16_t x = tsl.getLuminosity(TSL2591_VISIBLE);

  Serial.print(F("[ ")); Serial.print(millis()); Serial.print(F(" ms ] "));
  Serial.print(F("Luminosity: "));
  Serial.println(x, DEC);
}

int readBelysning(void)
{
  uint32_t lum = tsl.getFullLuminosity();
  uint16_t ir, full;
  ir = lum >> 16;
  full = lum & 0xFFFF;
  Serial.print(F("Visible: ")); Serial.print(full - ir); Serial.print(F("  "));
  Serial.print(F("Lux: ")); Serial.println(tsl.calculateLux(full, ir), 6);
  Serial.println("");
  delay(500);
  lux = tsl.calculateLux(full, ir);
  
  return lux;
}

int readJordFugtighed() {
  int sensorValue = analogRead(sensorPin);
  jordfugtighed = map(sensorValue, 0, 4095, 255, 0);
  analogWrite(ledPin, jordfugtighed);
  Serial.print("Analog output: ");
  Serial.println(jordfugtighed);
  Serial.println("");
  delay(500);

  return jordfugtighed;
}

int readTemp() {
  temp = bme.readTemperature();
    Serial.print("Temperature = ");
    Serial.print(temp);
    Serial.println(" °C");

    Serial.println();
    delay(500);
  return temp;
}

int readTryk()
{
  tryk = bme.readPressure() / 100.0F;

  Serial.print("Pressure = ");
    Serial.print(tryk);
    Serial.println(" hPa");

    Serial.println();
    delay(500);
    return tryk;
}

int readFugtighed()
{
  fugtighed = bme.readHumidity();
  Serial.print("Humidity = ");
    Serial.print(fugtighed);
    Serial.println(" %");
 
    Serial.println();
    delay(500);
  return fugtighed;
}

int readCo2()
{
  co2 = ccs.geteCO2();

  if(ccs.available()){
    if(!ccs.readData()){
      Serial.print("CO2: ");
      Serial.print(ccs.geteCO2());
      Serial.print("ppm, TVOC: ");
      Serial.println(ccs.getTVOC());
      Serial.println("");
    }
  else{
    Serial.println("ERROR!");
    while(1);
  }
  }
  delay(150);
  return co2;
}

int readVandSensor()
{
  vandmaengde = analogRead(35);
  Serial.println(String("Vand mængde: ") + vandmaengde);
  Serial.println();  
  delay(100);

  return vandmaengde;
}

void forloebetTid()
{


}

void setup(void) 
{
  Serial.begin(9600);

  connectWiFi();

  connectAzure();

  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, LOW);

  startTSL2591();
  
  startBME();

  startCSS();
}

void loop(void) 
{  
  lux           = readBelysning();
  jordfugtighed = readJordFugtighed();
  temp          = readTemp();
  tryk          = readTryk();
  fugtighed     = readFugtighed();
  co2           = readCo2();
  vandmaengde   = readVandSensor();

  mqtt.loop();

  postOnce();

  delay(60000);
}