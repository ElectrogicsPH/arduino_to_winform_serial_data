
const uint8_t bottle_counter = 1;

//
void sendCounterValue(int value)
{
  Serial.print("count=");
  Serial.println(value);
}

void setup()
{
  // Initialize Serial Connection
  Serial.begin(9600);
}

void loop()
{
  // send data every 2seconds
  sendCounterValue(bottle_counter);
  delay(2000);
}
