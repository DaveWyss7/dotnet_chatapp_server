class ChatApp {
    constructor() {
        this.connection = null;
        this.currentUser = null;
        this.currentRoomId = 1;
        this.authToken = null;
        this.typingTimer = null;
        this.init();
    }

    init() {
        this.bindEvents();
        this.addSystemMessage("Welcome! Please login to start chatting.");
    }

    bindEvents() {
        // Login
        document.getElementById('loginButton').addEventListener('click', () => this.login());
        document.getElementById('passwordInput').addEventListener('keypress', (e) => {
            if (e.key === 'Enter') this.login();
        });

        // Chat
        document.getElementById('joinRoomButton').addEventListener('click', () => this.joinRoom());
        document.getElementById('sendButton').addEventListener('click', () => this.sendMessage());
        document.getElementById('messageInput').addEventListener('keypress', (e) => {
            if (e.key === 'Enter') this.sendMessage();
        });

        // Typing indicator
        document.getElementById('messageInput').addEventListener('input', () => this.handleTyping());
    }

    async login() {
        const username = document.getElementById('usernameInput').value;
        const password = document.getElementById('passwordInput').value;
        const authStatus = document.getElementById('authStatus');

        try {
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password })
            });

            if (response.ok) {
                const data = await response.json();
                this.authToken = data.data.token;
                this.currentUser = { 
                    username: data.data.username, 
                    userId: data.data.userId 
                };

                authStatus.innerHTML = `✅ Welcome, ${this.currentUser.username}!`;
                authStatus.style.background = '#d4edda';
                authStatus.style.color = '#155724';

                document.getElementById('currentUsername').textContent = this.currentUser.username;
                
                // Switch to chat view
                document.getElementById('loginSection').style.display = 'none';
                document.getElementById('chatSection').style.display = 'block';

                // Connect to SignalR
                await this.connectToHub();
            } else {
                const error = await response.json();
                authStatus.innerHTML = `❌ ${error.error}`;
                authStatus.style.background = '#f8d7da';
                authStatus.style.color = '#721c24';
            }
        } catch (error) {
            authStatus.innerHTML = `❌ Connection error: ${error.message}`;
            authStatus.style.background = '#f8d7da';
            authStatus.style.color = '#721c24';
        }
    }

    async connectToHub() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/chathub")
            .build();

        // Connection events
        this.connection.start().then(() => {
            this.updateConnectionStatus(true);
            this.addSystemMessage("🟢 Connected to chat server!");
        }).catch(err => {
            this.updateConnectionStatus(false);
            this.addSystemMessage(`🔴 Connection failed: ${err.toString()}`);
        });

        // Message events
        this.connection.on("ReceiveMessage", (data) => this.addMessage(data));
        this.connection.on("UserOnline", (data) => this.handleUserOnline(data));
        this.connection.on("UserOffline", (data) => this.handleUserOffline(data));
        this.connection.on("UserJoinedRoom", (data) => this.handleUserJoinedRoom(data));
        this.connection.on("UserLeftRoom", (data) => this.handleUserLeftRoom(data));
        this.connection.on("TypingIndicator", (data) => this.handleTypingIndicator(data));
        this.connection.on("Error", (error) => this.addSystemMessage(`❌ ${error}`));
    }

    updateConnectionStatus(connected) {
        const status = document.getElementById('connectionStatus');
        if (connected) {
            status.innerHTML = '<span class="status-dot"></span>Connected';
            status.className = 'status connected';
        } else {
            status.innerHTML = '<span class="status-dot"></span>Disconnected';
            status.className = 'status disconnected';
        }
    }

    async joinRoom() {
        if (this.connection && this.currentUser) {
            try {
                await this.connection.invoke("JoinRoom", this.currentRoomId);
                this.addSystemMessage(`✅ Joined room: General`);
            } catch (err) {
                this.addSystemMessage(`❌ Error joining room: ${err.toString()}`);
            }
        }
    }

    async sendMessage() {
        const input = document.getElementById('messageInput');
        const content = input.value.trim();

        if (this.connection && this.currentUser && content) {
            try {
                const messageDto = {
                    content: content,
                    chatRoomId: this.currentRoomId
                };

                await this.connection.invoke("SendMessage", messageDto);
                input.value = '';
            } catch (err) {
                this.addSystemMessage(`❌ Error sending message: ${err.toString()}`);
            }
        }
    }

    addMessage(messageData) {
        const container = document.getElementById('messagesContainer');
        const messageDiv = document.createElement('div');
        messageDiv.className = 'message';

        const time = new Date(messageData.CreatedAt).toLocaleTimeString();
        messageDiv.innerHTML = `
            <div class="message-header">
                <span class="message-username">${messageData.Username}</span>
                <span class="message-time">${time}</span>
            </div>
            <div class="message-content">${this.escapeHtml(messageData.Content)}</div>
        `;

        container.appendChild(messageDiv);
        container.scrollTop = container.scrollHeight;
    }

    addSystemMessage(text) {
        const container = document.getElementById('messagesContainer');
        const messageDiv = document.createElement('div');
        messageDiv.className = 'system-message';
        messageDiv.innerHTML = `<span class="message-icon">🤖</span>${text}`;
        container.appendChild(messageDiv);
        container.scrollTop = container.scrollHeight;
    }

    handleUserOnline(userData) {
        this.addSystemMessage(`🟢 ${userData.Username} is now online`);
        this.updateOnlineUsers();
    }

    handleUserOffline(userData) {
        this.addSystemMessage(`🔴 ${userData.Username} went offline`);
        this.updateOnlineUsers();
    }

    handleUserJoinedRoom(data) {
        this.addSystemMessage(`👋 ${data.Username} joined the room`);
    }

    handleUserLeftRoom(data) {
        this.addSystemMessage(`👋 ${data.Username} left the room`);
    }

    handleTypingIndicator(data) {
        const indicator = document.getElementById('typingIndicator');
        const userSpan = document.getElementById('typingUser');

        if (data.IsTyping) {
            userSpan.textContent = data.Username;
            indicator.style.display = 'flex';
            
            // Hide after 3 seconds
            setTimeout(() => {
                indicator.style.display = 'none';
            }, 3000);
        } else {
            indicator.style.display = 'none';
        }
    }

    handleTyping() {
        if (this.connection && this.currentUser) {
            this.connection.invoke("SendTypingIndicator", this.currentRoomId, true);
            
            clearTimeout(this.typingTimer);
            this.typingTimer = setTimeout(() => {
                this.connection.invoke("SendTypingIndicator", this.currentRoomId, false);
            }, 1000);
        }
    }

    async updateOnlineUsers() {
        try {
            const response = await fetch('/api/chat/online-users');
            const users = await response.json();
            
            const usersList = document.getElementById('onlineUsersList');
            usersList.innerHTML = users.map(user => `
                <div class="user-item">
                    <span class="user-dot online"></span>
                    ${user.Username}
                </div>
            `).join('');
        } catch (error) {
            console.error('Error updating online users:', error);
        }
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
}

// Initialize app when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new ChatApp();
});