import random
import time

class User:
    def __init__(self, uid: int, ratings: dict):
        self.uid = uid
        self.ratings = ratings
        self.bucket = None

    def diffThresh(self, other: 'User', sample: list[int]) -> float:
        # t: total difference, s: stories considered
        if len(self.ratings) < 10:
            raise Exception("Users must have rated at least 10 stories to be eligible for Recommendations")
        a = [abs(self.ratings[i] - other.ratings[i]) for i in sample if i in self.ratings and i in other.ratings]
        if len(a) == 0:
            return 0
        return round(sum(a) / len(a), 3)

    def getStories(self, num: int):
        return a if len((a := [i for i in self.bucket.stories if i not in self.ratings])) <= num else a[:num]


class Bucket:
    def __init__(self, users: list['User']):
        self.users = users
        self.stories = []

    def __len__(self):
        return len(self.users)

    def split(self, sample: list[int]):
        if len(self) <= 128:  # MODIFY
            self.stories = self.findStories()
            buckets.append(self)
            for i in self.users:
                i.bucket = self
            return
        pivot = random.choice(self.users)
        self.users = [i for i in self.users if i is not pivot]
        sortedUsers = [pivot] + sorted([i for i in self.users], key=lambda x: x.diffThresh(pivot, sample))
        split = len(sortedUsers) // 2
        Bucket(sortedUsers[:split]).split(genSample(50))
        Bucket(sortedUsers[split:]).split(genSample(50))

    def findStories(self):
        return a if len(
            (a := sorted([i for i in stories if self.getAvg(i) != 0], key=lambda x: self.getAvg(x)))) <= 100 else a[
                                                                                                                  :100]

    def getAvg(self, story):
        return sum(a) / len(a) if len((a := [i.ratings[story] for i in self.users if story in i.ratings])) >= 16 else 0
