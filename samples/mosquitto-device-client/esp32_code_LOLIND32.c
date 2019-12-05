/*

  MQTT Subscribe Example with TLS-PSK
  ===========================
  Ulf Jennehag
  Use board: "ESP32 Dev Module"
  Can give errors during upload, try again, or try to hold down the "boot" button
  
  https://randomnerdtutorials.com/solved-failed-to-connect-to-esp32-timed-out-waiting-for-packet-header/

  LOLIN D32 board
  Use board: "LOLIN D32"
  Use lover speed on transfer i.e. 115200 and 40 MHz if you have problem with upload. (Needs to be investigated...)

  
*/

#include <WiFiClientSecure.h>
#include <PubSubClient.h>

const char* ssid     = "<ssid>";     // your network SSID (name of wifi network)
const char* password = "<password>"; // your network password

const IPAddress server = IPAddress(0,0,0,-1);  // Server IP address
const int    port = 8884; // server's port (8883 for MQTT), I run 8884 for MQTT-TLS-PSK

const char*  pskIdent = "<psk>"; // PSK identity (sometimes called key hint)
const char*  psKey = "<key>"; // PSK Key (must be hex string without 0x)

const char*  publishTopic = "<topic>";
const char*  subscribeTopic = "<topic>";

WiFiClientSecure networkClient;
PubSubClient client(networkClient);

// Send interval for the temperature sensor
const int sendInterval = 5000;
// Timer..
unsigned long delayTimer = millis();

// For reading temp sensor.
int tempVal;
float volts;
float temperature;

int toggleLed; //TODO:

// Allocate memory for string
char publishString[15];

// Define callback for the incoming messages. 
void callback(char* topic, byte* payload, unsigned int length) {

  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();
  
}

void setup() {
  
  //Initialize serial and wait for port to open:
  Serial.begin(115200);
  delay(100);

  // ====================================================
  Serial.print("Attempting to connect to SSID: ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);
    // attempt to connect to Wifi network:
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    // wait 1 second for re-trying
    delay(1000);
  }

  Serial.print("Connected to ");
  Serial.println(ssid);

  // Connected to WiFi
  // ====================================================
  
  // Set pskIdent and psKey
  Serial.println("Setting pskIdent and psKey.");
  networkClient.setPreSharedKey(pskIdent, psKey);

  // ====================================================

  // MQTT stuff
  // ====================================================
  Serial.println("Setting server and port.");
  client.setServer(server,port);
  Serial.println("Setting callback.");
  client.setCallback(callback);

  while (!client.connected()) {
    Serial.println("Connecting to MQTT...");
     if (client.connect("ESP32Client")) {
       Serial.println("connected");  
 
    } else {
 
      Serial.print("failed with state ");
      Serial.print(client.state());
      delay(2000);
 
    }
  }

  Serial.println("Setting topic");
  client.subscribe(subscribeTopic);
    
  
}

void loop() {

  client.loop();


  // With this example we avoid unessesary waiting, delaying client.loop();
  if ((millis()-delayTimer)>sendInterval) {
    // Ok, time to send temperature reading.
    
    tempVal = analogRead(34);
    Serial.print("TempVal:");
    Serial.print(tempVal);
    volts = (tempVal/4095.0)*5.0;
    Serial.print("  Voltage:");
    Serial.print(volts);    
    temperature = (volts-0.52)*100;
    Serial.print("  Temperature:");
    Serial.println(temperature);
    // Convert float to string
    dtostrf(temperature,5,2,publishString);

    client.publish(publishTopic,publishString);
        
    delayTimer = millis();
  }
    
  
}
