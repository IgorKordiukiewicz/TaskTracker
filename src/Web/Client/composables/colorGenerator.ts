export const useColorGenerator = () => {
    return {
        generateAvatarColor() {
            const colors = [ 
                '#10b981', '#22c55e', '#84cc16', '#ef4444', '#f97316', 
                '#f59e0b', '#eab308', '#14b8a6', '#06b6d4', '#0ea5e9', 
                '#3b82f6', '#6366f1', '#8b5cf6', '#a855f7', '#d946ef',
                '#ec4899', '#f43f5e' ];

            return colors[Math.floor(Math.random() * colors.length)];
        }
    }
}