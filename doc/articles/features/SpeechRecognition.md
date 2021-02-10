# Speech Recognition in Uno

Uno's implementation currently supports basic native speech recognition. 

## Supported Features

Following features of `Windows.Media.SpeechRecognition.SpeechRecognizer` are currently supported:

| Feature    									| iOS	| Android	| Remarks			|
|-----------------------------------------------|-------|-----------|-------------------|
| SpeechRecognizer()  							| X     | X  		|					|
| SpeechRecognizer(Language)  					| X     | X  		|					|
| Constraints 									| -     | -  		|					|
| ContinuousRecognitionSession  				| -     | -  		|					|
| CurrentLanguage  								| X     | X  		|					|
| State  										| X     | X  		|					|
| SupportedGrammarLanguages  					| -     | -  		|					|
| SupportedTopicLanguages  						| -     | -  		|					|
| SystemSpeechLanguage  						| -     | -  		|					|
| Timeouts  									| X     | X  		|					|
| UIOptions  									| X     | X  		| Not used			|
| CompileConstraintsAsync()  					| X     | X  		| Always return Success (implemented to meet UWP constraint that requires `CompileConstraintsAsync()` to be called before `RecognizeAsync()`)|
| Dispose()  									| X     | X  		|					|
| RecognizeAsync()  							| X     | X  		|					|
| RecognizeWithUIAsync()  						| -     | -  		|					|
| StopRecognitionAsync()()  					| X     | X  		|					|
| TrySetSystemSpeechLanguageAsync(Language)		| -     | -  		|					|
| HypothesisGenerated  							| X     | X  		|					|
| RecognitionQualityDegrading  					| -     | -  		|					|
| StateChanged  								| X     | X  		|					|

## Requirement

### iOS

__Requires iOS10+__
Following lines need to be added to your info.plist
```xml
<key>NSSpeechRecognitionUsageDescription</key>  
<string>[SpeechRecognition usage description]</string>  
<key>NSMicrophoneUsageDescription</key>  
<string>[SpeechRecognition usage description]</string> 
```

### Android

Following lines need to be added to your AndroidManifest.xml
```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
```

## Limitation

In `Windows.Media.SpeechRecognition.SpeechRecognitionResult`, only `Text`, `Alternates` and `GetAlternates(uint maxAlternates)` are implemented.
In particular, `RawConfidence` and `Confidence` fields are not currently supported
