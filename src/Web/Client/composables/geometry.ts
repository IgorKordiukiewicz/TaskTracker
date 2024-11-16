export const useGeometry = () => {
    return {
        getIntersectionPointOnRectangle(left: number, top: number, width: number, height: number, x: number, y: number) {
            const right = left + width;
            const bottom = top + height;
            const midX = (left + right) / 2;
            const midY = (top + bottom) / 2;
                
            const m = (midY - y) / (midX - x);
                
            if (x <= midX) { // left
	        	var minXy = m * (left - x) + y;
	        	if (top <= minXy && minXy <= bottom)
	        		return [left,minXy];
	        }
        
	        if (x >= midX) { // right
	        	var maxXy = m * (right - x) + y;
	        	if (top <= maxXy && maxXy <= bottom)
	        		return [right, maxXy];
	        }
        
	        if (y <= midY) { // top
	        	var minYx = (top - y) / m + x;
	        	if (left <= minYx && minYx <= right)
	        		return [minYx, top];
	        }
        
	        if (y >= midY) { // bottom
	        	var maxYx = (bottom - y) / m + x;
	        	if (left <= maxYx && maxYx <= right)
	        		return [maxYx, bottom];
	        }
        
	        // edge case when finding midpoint intersection: m = 0/0 = NaN
	        if (x === midX && y === midY) return [x, y];
        
            return [,];
        }
    }
}