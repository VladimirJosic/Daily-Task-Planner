export const API_URL = "/api";

// ai
export const queryLLM = `${API_URL}/Ai/query-llm`;
export const clearLLMChatHistory = `${API_URL}/Ai/clear-chat-history`;

// auth
export const login = `${API_URL}/Auth/login`;
export const requestPasswordReset = `${API_URL}/Auth/reset-password`;
export const setNewPassword = `${API_URL}/Auth/set-new-password`;

// users
export const getAllUsers = `${API_URL}/Users/get-all`;
export const getMe = `${API_URL}/Users/me`;

// dailyTask
export const createTask = `${API_URL}/DailyTask/create`;
export const getAllTasks = `${API_URL}/DailyTask/get-all`;
export const getAllTasksDateRange = `${API_URL}/DailyTask/get-all-date-range`;
export const deactivateTask = `${API_URL}/DailyTask/deactivate`;
export const shareTask = `${API_URL}/DailyTask/share`;
export const getSharedTasks = `${API_URL}/DailyTask/get-all-shared`;
export const searchTasks = `${API_URL}/DailyTask/search`;

export const deleteSharedTask = `${API_URL}/DailyTask/DeleteSharedTask`;
