﻿interface Window {
	SpeechRecognition: any;
	webkitSpeechRecognition: any;
}

namespace Windows.Media {
	export class SpeechRecognizer {

		private static dispatchResult:
			(instanceId: string, result: string, confidence: number) => number;

		private static dispatchHypothesis:
			(instanceId: string, hypothesis: string) => number;

		private static dispatchStatus:
			(instanceId: string, status: string) => number;

		private static dispatchError:
			(instanceId: string, error: string) => number;

		private static instanceMap: { [managedId: string]: SpeechRecognizer } = {};

		private managedId: string;
		private recognition: SpeechRecognition;

		private constructor(managedId: string, culture: string) {
			this.managedId = managedId;
			if (window.SpeechRecognition) {
				this.recognition = new window.SpeechRecognition(culture);
			}
			else if (window.webkitSpeechRecognition) {
				this.recognition = new window.webkitSpeechRecognition(culture);
			}
			if (this.recognition) {
				this.recognition.addEventListener("result", this.onResult);
				this.recognition.addEventListener("speechstart", this.onSpeechStart);
				this.recognition.addEventListener("error", this.onError);
			}
		}

		public static initialize(managedId: string, culture: string) {
			const recognizer = new SpeechRecognizer(managedId, culture);
			SpeechRecognizer.instanceMap[managedId] = recognizer;
		}

		public static recognize(managedId: string) : boolean {
			const recognizer = SpeechRecognizer.instanceMap[managedId];
			if (recognizer.recognition) {
				recognizer.recognition.continuous = false;
				recognizer.recognition.interimResults = true;
				recognizer.recognition.start();
				return true;
			} else {
				return false;
			}
		}

		public static removeInstance(managedId: string) {
			const recognizer = SpeechRecognizer.instanceMap[managedId];
			recognizer.recognition.removeEventListener("result", recognizer.onResult);
			recognizer.recognition.removeEventListener("speechstart", recognizer.onSpeechStart);
			recognizer.recognition.removeEventListener("error", recognizer.onError);
			delete SpeechRecognizer.instanceMap[managedId];
		}

		private onResult = (event: SpeechRecognitionEvent) => {
			if (event.results[0].isFinal) {
				if (!SpeechRecognizer.dispatchResult) {
					SpeechRecognizer.dispatchResult = (<any>Module).mono_bind_static_method(
						"[Uno] Windows.Media.SpeechRecognition.SpeechRecognizer:DispatchResult");
				}
				SpeechRecognizer.dispatchResult(this.managedId, event.results[0][0].transcript, event.results[0][0].confidence);
			} else {
				if (!SpeechRecognizer.dispatchHypothesis) {
					SpeechRecognizer.dispatchHypothesis = (<any>Module).mono_bind_static_method(
						"[Uno] Windows.Media.SpeechRecognition.SpeechRecognizer:DispatchHypothesis");
				}
				SpeechRecognizer.dispatchHypothesis(this.managedId, event.results[0][0].transcript);
			}
		}

		private onSpeechStart = () => {
			if (!SpeechRecognizer.dispatchStatus) {
				SpeechRecognizer.dispatchStatus = (<any>Module).mono_bind_static_method(
					"[Uno] Windows.Media.SpeechRecognition.SpeechRecognizer:DispatchStatus");
			}
			SpeechRecognizer.dispatchStatus(this.managedId, "SpeechDetected")
		}

		private onError = (event: SpeechSynthesisErrorEvent) => {
			if (!SpeechRecognizer.dispatchError) {
				SpeechRecognizer.dispatchError = (<any>Module).mono_bind_static_method(
					"[Uno] Windows.Media.SpeechRecognition.SpeechRecognizer:DispatchError");
			}
			SpeechRecognizer.dispatchError(this.managedId, event.error);
		}
	}
}
