export const useTimeParser = () => {
    return {
        tryToMinutes(input: string) {
            if (!input || input.trim().length === 0)
            {
                return { result: false, value: 0 };
            }
        
            // Treat integer input as hours ?
            const numberInput = Number(input);
            const isInteger = isNaN(numberInput) && numberInput % 1 == 0;
            if(isInteger)
            {
                return { result: true, value: numberInput * 60 };
            }
        
            // match negative numbers too to so method can fail instead of ignoring the group with it
            var regex = "((-*\\d+d)? *(-*\\d+h)? *(-*\\d+m)? *)";
            var match = input.match(regex);;

            // fail when match is shorter than input to prevent inputs in wrong order
            if(!match || match[0].length < input.length) {
                return { result: false, value: 0 };
            }
        
            var days = match[2] ?? '';
            var hours = match[3] ?? '';
            var minutes = match[4] ?? '';
            
            // fail if didn't match any groups or any group has a negative value
            if((!days && !hours && !minutes )|| days.startsWith('-') || hours.startsWith('-') || minutes.startsWith('-'))
            {
                return { result: false, value: 0 };
            }
        
            const result = (parseGroup(days) * 24 * 60) + (parseGroup(hours) * 60) + parseGroup(minutes);
            return { result: true, value: result };
        },
        fromMinutes(totalMinutes: number) {
            if (totalMinutes <= 0)
            {
                return "0h";
            }
            
            const days = Math.floor(totalMinutes / (24 * 60));
            const hours = Math.floor((totalMinutes % (24 * 60)) / 60);
            const minutes = totalMinutes % 60;
        
            let result = '';
            if (days > 0)
            {
                result += ` ${days}d`;
            }
            if (hours > 0)
            {
                result += ` ${hours}h`;
            }
            if (minutes > 0)
            {
                result += ` ${minutes}m`;
            }
        
            return result.trim();
        },
        toReadableTimeDifference(date: Date) {
            const from = new Date(date);
            const to = new Date(Date.now());
            if(from > to) {
                return '';
            }

            var diffInSeconds = Math.floor((to.valueOf() - from.valueOf()) / 1000);
            if(diffInSeconds < 10) {
                return "just now";
            }
            else if(diffInSeconds < 60) {
                return "a few seconds ago";
            }
            else if(diffInSeconds < 120) {
                return "a minute ago";
            }
            else if(diffInSeconds < 60 * 60) {
                return `${Math.floor(diffInSeconds / 60)} minutes ago`;
            }
            else if(diffInSeconds < 60 * 60 * 2) {
                return 'an hour ago';
            }
            else if(diffInSeconds < 60 * 60 * 24) {
                return `${Math.floor(diffInSeconds / 60 / 60)} hours ago`;
            }
            else {
                return new Date(date).toLocaleDateString();
            }
        }
    }

    function parseGroup(group: string) {
        return group ? Number(group.slice(0, -1)) : 0;
    }
}