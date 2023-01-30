int bottle_counter = 0;

void sendCounter(int _counter)
{
  Serial.print("count=");
  Serial.println(_counter);
}

void setup()
{
  Serial.begin(9600);
  sendCounter(bottle_counter);
}
void loop()
{
  sendCounter(bottle_counter);
  delay(3000);

  bottle_counter++;
}
