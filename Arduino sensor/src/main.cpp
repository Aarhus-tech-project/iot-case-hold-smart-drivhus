#include <SPI.h>
#include <LoRa.h>
#include <Wire.h>
#include <Adafruit_BME280.h>
#include <Adafruit_PWMServoDriver.h>
#include "Adafruit_TSL2591.h"
#include "Adafruit_CCS811.h"

//Motor
#define FREQ 60
#define STOP_US 1425
Adafruit_PWMServoDriver pwm(0x40);
const int CH = 15; 

//Lora 
#define SS 10
#define RST 9
#define DIO0 2

//Pumpe
const unsigned int IN1 = 7;
const unsigned int IN2 = 8;
const unsigned int EN = 6;

//Sensor 
#define ledPin 2
#define soilPin A5
#define waterPin A0

Adafruit_TSL2591 tsl = Adafruit_TSL2591(2591);
Adafruit_BME280 bme;
Adafruit_CCS811 ccs;

//payload variabler
int lux;
int temp;
int tryk;
int fugtighed;
int co2;
int vandmaengde;
int jordfugtighed;
String command;

//LoRa delay foranstaltninger
unsigned long lastSend = 0;
const unsigned long sendInterval = 60000;

void startCSS() {
  Serial.println("CCS811 init...");
  if (!ccs.begin()) {
    Serial.println("Failed to start CCS811! Check wiring.");
    while (1);
  }
  while (!ccs.available());
}

void startMotor() {
    pwm.begin();
  pwm.setOscillatorFrequency(25000000);
  pwm.setPWMFreq(FREQ);

  pwm.writeMicroseconds(CH, STOP_US); delay(1000); //nulstil motor
  delay(10);
  Serial.println("Motors started");
}

void setupPumpe() {
  pinMode(IN1, OUTPUT);
  pinMode(IN2, OUTPUT);
  pinMode(EN, OUTPUT);
  Serial.println("Pumpe started");
}

void startBME() {
  if (!bme.begin()) {
    Serial.println("BME280 not found!");
    while (1);
  }
  Serial.println("BME280 ready");
}

void configureTSL2591() {
  tsl.setGain(TSL2591_GAIN_MED);
  tsl.setTiming(TSL2591_INTEGRATIONTIME_300MS);
}

void motorHub(String command)
{
  Serial.println("motorhub entered");
  if (command == "openWindow")
  {
    pwm.writeMicroseconds(CH, 1000); delay(1000);
    pwm.writeMicroseconds(CH, STOP_US); delay(1000);
      Serial.println("window open");
  }
  else if (command == "closeWindow")
  {
    pwm.writeMicroseconds(CH, 2000); delay(1000);
    pwm.writeMicroseconds(CH, STOP_US); delay(1000);
      Serial.println("window closed");
  }
}

void pumpeHub(String command) {
  if (command == "startPumpe")
  {
  Serial.println("Pumpe running");
  digitalWrite(IN1, HIGH);
  digitalWrite(IN2, LOW);
  analogWrite(EN, 255);  // 0–255 kontrol
  delay(2000);

  Serial.println("Pumpe stopping");
  digitalWrite(IN1, LOW);
  digitalWrite(IN2, LOW);
  analogWrite(EN, 0); // 0-255 kontrol
  delay(2000);
  }
}

void startTSL2591() {
  if (tsl.begin()) {
    Serial.println("TSL2591 ready");
    configureTSL2591();
  } else {
    Serial.println("No TSL2591 found");
    while (1);
  }
}

void setupLoRa() {
    LoRa.setPins(SS, RST, DIO0);
  if (!LoRa.begin(433E6)) {
    Serial.println("Starting LoRa failed!");
    while (1);
  }
    Serial.println("LoRa ready");  
}

int readBelysning() {
  uint32_t lum = tsl.getFullLuminosity();
  uint16_t ir = lum >> 16;
  uint16_t full = lum & 0xFFFF;
  lux = tsl.calculateLux(full, ir);
  Serial.println(lux);
  return lux;
}

int readJordFugtighed() {
  int sensorValue = analogRead(soilPin);
  jordfugtighed = map(sensorValue, 0, 1023, 255, 0);
  analogWrite(ledPin, jordfugtighed);
  Serial.println(jordfugtighed);
  return jordfugtighed;
}

int readTemp() {
  Serial.println(bme.readTemperature());
  return bme.readTemperature(); 
}

int readTryk() { 
  Serial.println(bme.readPressure() / 100.0F);
  return bme.readPressure() / 100.0F; 
}

int readFugtighed() { 
  Serial.println(bme.readHumidity());
  return bme.readHumidity(); 
}

int readCo2() {
  if (ccs.available() && !ccs.readData()) 
  {
    co2 = ccs.geteCO2();
  }
  Serial.println(co2);
  return co2;
}

int readVandSensor() {
  vandmaengde = analogRead(waterPin);
  Serial.println(vandmaengde);
  return vandmaengde;
}

void setup() {
  Serial.begin(9600);

  // setup alle sensorer
  setupLoRa();
  setupPumpe();
  startMotor();
  startTSL2591();
  startBME();
  startCSS();

}

void loop() {
  // LoRa
  int packetSize = LoRa.parsePacket();
  if (packetSize) {
    String msg = "";
    while (LoRa.available()) {
      msg += (char)LoRa.read();
    }
    Serial.print("Received command: ");
    Serial.println(msg);

    if (msg == "Entity: Data: Event: Open window.") motorHub("openWindow");
    if (msg == "Entity: Data: Event: Close window.") motorHub("closeWindow");
    if (msg == "Entity: Data: Event: Soil Moisture low.") pumpeHub("startPumpe");
  }

  unsigned long now = millis();
  if (now - lastSend >= sendInterval) {
    lastSend = now;

    temp          = readTemp();
    tryk          = readTryk();
    fugtighed     = readFugtighed();
    co2           = readCo2();
    lux           = readBelysning();
    jordfugtighed = readJordFugtighed();
    vandmaengde   = readVandSensor();

    String payload = "{";
    payload += "\"temp\":" + String(temp) + ",";
    payload += "\"pressure\":" + String(tryk) + ",";
    payload += "\"humidity\":" + String(fugtighed) + ",";
    payload += "\"co2\":" + String(co2) + ",";
    payload += "\"lux\":" + String(lux) + ",";
    payload += "\"soil\":" + String(jordfugtighed) + ",";
    payload += "\"water\":" + String(vandmaengde);
    payload += "}";

    LoRa.beginPacket();
    LoRa.print("greenhousedata" + payload);
    LoRa.endPacket();

    Serial.print("Sent: ");
    Serial.println(payload);
  }
}