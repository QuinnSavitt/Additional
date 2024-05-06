# Leetcode daily 5/2/2024, find largest integer k such that -k is also in array nums
def findMaxK(nums: List[int]) -> int:
        return max(r) if (r := [i for i in nums if i>0 and i*-1 in [j for j in nums if j < 0]]) else -1

# Leetcode Problem 1051. Wastes space by continuously resorting s, a two line solution would be more optimal (and it is what I have on Leetcode!) but for the purpose of CodeGolf it is one.
# Checks how many heights are not in correct sorted order
def heightChecker(self, heights: List[int]) -> int:
        return sum([1 for i in range(len(heights)) if heights[i] != sorted(heights)[i]])

# Leetcode Problem 1030. Beats 81% Runtime, 61% Memory while also being CodeGolf
# Sort all cells in a Matrix of size [rows, cols] by Manhattan distance to [rCenter, cCenter]
def allCellsDistOrder(self, rows: int, cols: int, rCenter: int, cCenter: int) -> List[List[int]]:
        return sorted([[i, j] for i in range(rows) for j in range(cols)], key=lambda a : abs(a[0]-rCenter) + abs(a[1] - cCenter))

# Leetcode Problem 1304. Could be so much more efficient but for the purpose of CodeGolf it works. Beat 80% on memory though
# Create an array of size n with unique integers that sum to 0
def sumZero(self, n: int) -> List[int]:
        return [i for i in range(1, n//2+1)] + [-i for i in range(1, n//2+1)] + ([0] if n%2 else [])

# Leetcode Problem 1189. I don't even want to talk about how slow this is.
# Find the maximum number of times the word "balloon" can be formed with the letters of text.
def maxNumberOfBalloons(self, text: str) -> int:
        return min({"b":sum(1 for i in t if i == 'b'), "a":sum(1 for i in t if i == "a"), "l":sum(1 for i in t if i == 'l')//2, "o":sum(1 for i in t if i == 'o')//2, "n":sum(1 for i in t if i == 'n')}.values())
