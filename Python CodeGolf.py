# Leetcode daily 5/2/2024, find largest integer k such that -k is also in array nums
def findMaxK(nums: List[int]) -> int:
        return max(r) if (r := [i for i in nums if i>0 and i*-1 in [j for j in nums if j < 0]]) else -1

# Leetcode Problem 1051. Wastes space by continuously resorting s, a two lin.e solution would be more optimal (and it is what I have on Leetcode!) but for the purpose of CodeGolf it is one.
# Checks how many heights are not in correct sorted order
def heightChecker(self, heights: List[int]) -> int:
        return sum([1 for i in range(len(heights)) if heights[i] != sorted(heights)[i]])

# Leetcode Problem 1030. Beats 81% Runtime, 61% Memory while also being CodeGolf.
# Sort all cells in a Matrix of size [rows, cols] by Manhattan distance to [rCenter, cCenter].
def allCellsDistOrder(self, rows: int, cols: int, rCenter: int, cCenter: int) -> List[List[int]]:
        return sorted([[i, j] for i in range(rows) for j in range(cols)], key=lambda a : abs(a[0]-rCenter) + abs(a[1] - cCenter))

# Leetcode Problem 1304. Could be so much more efficient but for the purpose of CodeGolf it works. Beat 80% on memory though.
# Create an array of size n with unique integers that sum to 0.
def sumZero(self, n: int) -> List[int]:
        return [i for i in range(1, n//2+1)] + [-i for i in range(1, n//2+1)] + ([0] if n%2 else [])

# Leetcode Problem 1189. I don't even want to talk about how slow this is.
# Find the maximum number of times the word "balloon" can be formed with the letters of text.
def maxNumberOfBalloons(self, text: str) -> int:
        return min({"b":sum(1 for i in t if i == 'b'), "a":sum(1 for i in t if i == "a"), "l":sum(1 for i in t if i == 'l')//2, "o":sum(1 for i in t if i == 'o')//2, "n":sum(1 for i in t if i == 'n')}.values())

# LeetCode Problem 1365. Do I have to even say it anymore? It's slow. On the plus side, 87th percentile for memory utilization though.
# Find the number of elements in the array smaller than each element in the array.
def smallerNumbersThanCurrent(self, nums: List[int]) -> List[int]:
        return [sum(1 for i in range(len(nums)) if i != j and nums[i]<nums[j]) for j in range(len(nums))]

# LeetCode Problem 1295. Not a fun CodeGolf, but a CodeGolf nonetheless. Beat 71% Runtime, 96% Memory.
# Find how many numbers in an array have an even number of digits.
def findNumbers(self, nums: List[int]) -> int:
        return sum(1 for i in nums if not len(str(i))%2)

# LeetCode Problem 1374. Is it a CodeGolf if doing it in more lines would be wrong? Beats 61% Runtime, 66% Memory.
# Return a string of len n with only odd Character Counts
def generateTheString(self, n: int) -> str:
        return "a"*(n-1)+"b" if not n%2 else "a"*n

# LeetCode Problem 2150. Two lines (assigning the Counter to a variable) is the more optimal solution, and you can find that solution on my Leetcode, but then it wouldn't be CodeGolf.
# Find all "lonely numbers" (numbers that only exist once and neither neighbor are present)
def findLonely(self, nums: List[int]) -> List[int]:
        return [i for i in Counter(nums) if Counter(nums)[i] == 1 and i-1 not in Counter(nums) and i+1 not in Counter(nums)]

# LeetCode Problem 654. More optimal solution at 3 lines. This might be the most horrific beast in this file, and therefore it is my greatest accomplishment. Sadly Slow, but 79% Memory.
# Make a "Maximum Binary Tree" from an array of integers.
def constructMaximumBinaryTree(self, nums: List[int]) -> Optional[TreeNode]:
        return TreeNode((m := max(nums)), constructMaximumBinaryTree(nums[:(i := nums.index(m))]), constructMaximumBinaryTree(nums[i+1:])) if nums else None
