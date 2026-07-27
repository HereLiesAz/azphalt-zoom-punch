/*{
  "DESCRIPTION": "Radial motion blur streaking out from centre, pulsing in and out \u2014 the impact hit used on drops and reveals.",
  "CATEGORIES": ["Guillotine", "Distortion"],
  "INPUTS": [
  {
    "NAME": "inputImage",
    "TYPE": "image"
  },
  {
    "NAME": "amount",
    "TYPE": "float",
    "DEFAULT": 0.45,
    "MIN": 0.0,
    "MAX": 1.0
  },
  {
    "NAME": "speed",
    "TYPE": "float",
    "DEFAULT": 1.0,
    "MIN": 0.0,
    "MAX": 3.0
  }
]
}*/
void main() {
  vec2 uv = isf_FragNormCoord;
  vec2 centered = uv - vec2(0.5);

  // A pulsing envelope so it breathes; set speed to 0 for a constant, static zoom blur.
  float pulse = speed > 0.001 ? (0.5 + 0.5 * sin(TIME * speed * 3.0)) : 1.0;
  float strength = amount * pulse * 0.12;

  // March samples back toward centre — each step slightly less zoomed — and average.
  vec3 col = vec3(0.0);
  for (int i = 0; i < 10; i++) {
    float s = 1.0 - strength * (float(i) / 10.0);
    col += IMG_NORM_PIXEL(inputImage, centered * s + vec2(0.5)).rgb;
  }
  col /= 10.0;

  // Keep the centre sharp so the subject survives; only the edges streak.
  float keep = 1.0 - smoothstep(0.05, 0.55, length(centered));
  vec4 c = IMG_THIS_PIXEL(inputImage);
  gl_FragColor = vec4(mix(col, c.rgb, keep), c.a);
}
