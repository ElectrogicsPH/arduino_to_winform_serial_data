
int input = 4; // GPIO4
int state = 0;

#define DETECTED LOW  // change to HIGH if sensor produce Logic high when detected

const int bottle_count = 1;

void sendCounter(int _counter)
{
  Serial.print("count=");
  Serial.println(_counter);
}

void setup()
{
  delay(1000); // give time to boot completely
  // Initialize serial communication
  Serial.begin(9600);
  pinMode(input, INPUT_PULLUP);
}
void loop()
{
  bool inputState = digitalRead(input);
  if (inputState == DETECTED && state == 0)
  {
    // send count=1
    sendCounter(bottle_count);
    state = 1;
    delay(1000); // remove if not needed
  }
  else if (inputState == !DETECTED && state == 1)
  {
    state = 0;
  }
}
