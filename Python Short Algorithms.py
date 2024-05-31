# Inspired by LeetCode's Climbing Stairs Challenge, how many combinations of stairs can you take in a staircase of length 
class StairClimbing:
  stored = {0:0, 1:1, 2:2}
  def climbStairs(self, n):
          return self.stairHelper(n)
  def stairHelper(self, n):
      if n in self.stored:
          return self.stored[n]
      add = self.stairHelper(n-1) + self.stairHelper(n-2)
      self.stored[n] = add
      return add

# While not the Optimal Approach (will be added next), a beautiful example (in my humble opinion) of using a binary search and a 2D sliding window to solve LeetCode problem 221.
# Find the maximum area of a square containing only 1s
def maximalSquare(self, matrix: List[List[str]]) -> int:
        matrix = list(map(lambda a : list(map(int, a)), matrix))
        if not any(list(map(any, matrix))):
            return 0
        h, w = len(matrix), len(matrix[0])
        square = min(w, h)
        change = square
        checked = {}
        while change > 4:
            found = False
            for y in range(0, h-square+1):
                if found:
                    break
                for x in range(0, w-square+1):
                    if all(list(map(all, [matrix[i][x:x+square] for i in range(y, y+square)]))):
                        if square == min(h, w):
                            return square*square
                        found = True
                        break
            change = change//2
            square = square+change if found else square-change
        for i in range(square+2, max(square-3, 0), -1):
            for y in range(0, h-i+1):
                for x in range(0, w-i+1):
                    if all(list(map(all, [matrix[j][x:x+i] for j in range(y, y+i)]))):
                        return i**2
        return max(square-3, 1)**2
  #Optimal Version using dynamic programming:
  def maximalSquareOptimal(self, matrix: List[List[str]]) -> int:
        dp = [[0]*len(matrix[0]) for i in range(len(matrix))]
        r = 0
        for y in range(len(matrix)):
            for x in range(len(matrix[0])):
                if not x or not y:
                    dp[y][x] = int(matrix[y][x])
                elif matrix[y][x] == '1':
                    dp[y][x] = min(dp[y-1][x], dp[y-1][x-1], dp[y][x-1]) + 1
                r = max(r, dp[y][x])
        return r**2

# Fun LeetCode daily challenge on 5/6/2024. Used a stack approach and achieved 73.5% Runtime, 64% Memory.
# Double an integer represented by a singly linked list and return the result in the same form.
def doubleIt(self, head: Optional[ListNode]) -> Optional[ListNode]:
        if not head.next:
            if head.val < 5:
                head.val *= 2
                return head
            else:
                t = head.val*2
                head.val = t//10
                head.next = ListNode(t%10)
                return head
        it = head
        stack = deque()
        while it:
            stack.append(it)
            it = it.next
        carry = False
        while stack:
            a = stack.pop()
            a.val = a.val*2 + 1 if carry else a.val*2
            carry = a.val > 9
            a.val %= 10
        return ListNode(1, head) if carry else head

# LeetCode Contest 3/31/2024 Problem 3. Simple Dynamic Programming Approach. Beats 86.6% RunTime, 98% Memory.
# Count the total number of contiguous alternating subarrays.
def countAlternatingSubarrays(self, nums: List[int]) -> int:
        dp = [1]
        for i in range(1, len(nums)):
            if nums[i] != nums[i-1]:
                dp.append(1+dp[i-1])
            else:
                dp.append(1)
        return sum(dp)
