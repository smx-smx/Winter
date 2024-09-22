declare global {
	interface External {
		receiveMessage: (callback: (message: string) => void) => void;
		sendMessage: (message: string) => void;
	}
}

export {};