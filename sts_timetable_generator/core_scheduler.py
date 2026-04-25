import random
import statistics
from itertools import combinations
from collections import defaultdict


# ==============================
# MAIN WRAPPER FUNCTION
# ==============================

def run_tournament_scheduler(config):

    random.seed(config.get("seed",42))

    teams = config["teams"]
    venues = config["venues"]
    time_slots = config["daily_slots"]

    if isinstance(config["days"], int):
        days = [f"Day {i}" for i in range(1, config["days"]+1)]
    else:
        days = config["days"]

    matches = list(combinations(teams,2))

    NUM_ISLANDS = 4
    MIGRATION_INTERVAL = 10
    MIGRATION_COUNT = 2


    # =========================================
    # Population
    # =========================================

    def all_possible_slots():
        slots=[]
        for day in days:
            for time_slot in time_slots:
                for venue in venues:
                    slots.append(
                        (day,time_slot,venue)
                    )
        return slots


    def create_individual():

        schedule=[]

        available_slots=all_possible_slots()

        random.shuffle(available_slots)

        if len(matches)>len(available_slots):
            raise Exception(
                "Too many matches for available slots!"
            )

        used_slots=set()
        team_played_day=defaultdict(set)

        matches_copy=matches.copy()
        random.shuffle(matches_copy)

        for match in matches:

            assigned=False

            for slot in available_slots:

                day,time_slot,venue=slot

                if slot in used_slots:
                    continue

                team1,team2=match

                if (day in team_played_day[team1]) or \
                   (day in team_played_day[team2]):
                    continue

                schedule.append(
                    (
                        team1,
                        team2,
                        day,
                        time_slot,
                        venue
                    )
                )

                used_slots.add(slot)

                team_played_day[team1].add(day)
                team_played_day[team2].add(day)

                assigned=True
                break

            if not assigned:
                raise Exception(
                    "Couldn't assign all matches without conflicts!"
                )

        return schedule


    def create_population(size):

        population=[]

        while len(population)<size:
            individual=create_individual()

            if individual not in population:
                population.append(individual)

        return population



    # =========================================
    # Fitness
    # =========================================

    def fitness_function(schedule):

        penalty=0
        bonus=0

        team_schedule=defaultdict(list)
        venue_schedule=defaultdict(list)

        for match in schedule:

            team1,team2,day,time_slot,venue=match

            day_index=days.index(day)

            team_schedule[team1].append(
                (day_index,time_slot)
            )

            team_schedule[team2].append(
                (day_index,time_slot)
            )

            venue_schedule[
                (day,time_slot,venue)
            ].append(
                (team1,team2)
            )

        for team,plays in team_schedule.items():

            plays.sort()

            played_days=[
                d for d,_ in plays
            ]

            if len(set(played_days))<len(played_days):
                penalty+=50

            for i in range(
                1,
                len(played_days)
            ):
                if played_days[i]-played_days[i-1]==1:
                    penalty+=20

            if len(set(played_days))>1:
                try:
                    var=statistics.variance(
                        played_days
                    )
                    bonus+=var*0.5
                except:
                    pass

        for slot,matches_at_slot in venue_schedule.items():

            if len(matches_at_slot)>1:
                penalty+=100*(
                    len(matches_at_slot)-1
                )

        return max(
            0,
            1000-penalty+bonus
        )



    # =========================================
    # GA Operators
    # =========================================

    def tournament_selection(
            population,
            pool_size,
            tournament_size=5
    ):

        mating_pool=[]

        for _ in range(pool_size):

            tournament=random.sample(
                population,
                tournament_size
            )

            fitnesses=[
                fitness_function(ind)
                for ind in tournament
            ]

            winner=tournament[
                fitnesses.index(
                    max(fitnesses)
                )
            ]

            mating_pool.append(
                winner
            )

        return mating_pool



    def order_crossover(
        parent1,
        parent2,
        crossover_rate=0.8
    ):

        if random.random()>=crossover_rate:
            return parent1,parent2


        point1=random.randint(
            0,
            len(parent1)-1
        )

        point2=random.randint(
            point1+1,
            len(parent1)
        )


        segment=parent1[
            point1:point2
        ]

        offspring1=[
            m for m in parent2
            if m not in segment
        ]

        offspring1=(
            offspring1[:point1]
            +segment+
            offspring1[point1:]
        )


        segment2=parent2[
            point1:point2
        ]

        offspring2=[
            m for m in parent1
            if m not in segment2
        ]

        offspring2=(
            offspring2[:point1]
            +segment2+
            offspring2[point1:]
        )

        return offspring1,offspring2



    def swap_mutation(
            schedule,
            mutation_rate=0.1
    ):

        if random.random()<mutation_rate:

            idx1,idx2=random.sample(
                range(len(schedule)),
                2
            )

            schedule[idx1],schedule[idx2]=\
            schedule[idx2],schedule[idx1]


            match_to_mutate=random.choice(
                schedule
            )

            match_to_mutate=list(
                match_to_mutate
            )

            match_to_mutate[2]=random.choice(
                days
            )

            match_to_mutate[4]=random.choice(
                venues
            )

            for i in range(
                len(schedule)
            ):

                if schedule[i][0]==match_to_mutate[0]\
                   and schedule[i][1]==match_to_mutate[1]:

                    schedule[i]=tuple(
                        match_to_mutate
                    )

                    break

        return schedule



    def survivor_selection(
        population,
        offspring
    ):

        combined=population+offspring

        combined_sorted=sorted(
            combined,
            key=lambda ind:
            fitness_function(ind),
            reverse=True
        )

        return combined_sorted[
            :len(population)
        ]



    def create_islands(
        pop_size,
        num_islands
    ):
        island_size=pop_size//num_islands

        return [
            create_population(
                island_size
            )
            for _ in range(
                num_islands
            )
        ]



    # =========================================
    # Memetic Search
    # =========================================

    def local_search(
        individual,
        probability=0.2
    ):

        if random.random()<probability:

            new_individual=individual[:]

            i,j=random.sample(
                range(
                    len(new_individual)
                ),
                2
            )

            new_individual[i],new_individual[j]=\
            new_individual[j],new_individual[i]

            if fitness_function(
                new_individual
            )>fitness_function(
                individual
            ):
                return new_individual

        return individual



    # =========================================
    # Main Island GA
    # =========================================

    def run_island_genetic_algorithm():

        population_size=config.get(
            "population_size",
            40
        )

        max_generations=config.get(
            "max_generations",
            5000
        )

        mutation_rate=config.get(
            "mutation_rate",
            0.4
        )

        crossover_rate=config.get(
            "crossover_rate",
            0.9
        )

        local_search_prob=config.get(
            "local_search_prob",
            0.4
        )


        islands=create_islands(
            population_size,
            NUM_ISLANDS
        )

        for i in range(NUM_ISLANDS):
            for j in range(
                len(islands[i])
            ):
                islands[i][j]=local_search(
                    islands[i][j],
                    local_search_prob
                )


        best_overall=None
        best_fitness=float("-inf")
        fitness_history=[]


        for generation in range(
            max_generations
        ):

            for i in range(NUM_ISLANDS):

                population=islands[i]
                offspring=[]

                for _ in range(
                    len(population)//2
                ):

                    parent1,parent2=\
                    tournament_selection(
                        population,
                        2,
                        3
                    )

                    if random.random()<crossover_rate:

                        child1,child2=\
                        order_crossover(
                            parent1,
                            parent2,
                            crossover_rate
                        )
                    else:
                        child1=parent1[:]
                        child2=parent2[:]


                    child1=swap_mutation(
                        child1,
                        mutation_rate
                    )

                    child2=swap_mutation(
                        child2,
                        mutation_rate
                    )

                    child1=local_search(
                        child1,
                        local_search_prob
                    )

                    child2=local_search(
                        child2,
                        local_search_prob
                    )

                    offspring.extend(
                        [child1,child2]
                    )


                islands[i]=survivor_selection(
                    population,
                    offspring
                )


            if generation%MIGRATION_INTERVAL==0\
               and generation>0:

                for i in range(NUM_ISLANDS):

                    source=islands[i]
                    target=islands[
                        (i+1)%NUM_ISLANDS
                    ]

                    migrants=sorted(
                        source,
                        key=lambda ind:
                        fitness_function(ind),
                        reverse=True
                    )[:MIGRATION_COUNT]


                    replacees=sorted(
                        target,
                        key=lambda ind:
                        fitness_function(ind)
                    )[:MIGRATION_COUNT]


                    for m,r in zip(
                        migrants,
                        replacees
                    ):
                        idx=target.index(r)
                        target[idx]=m



            for pop in islands:

                best=max(
                    pop,
                    key=fitness_function
                )

                fit=fitness_function(
                    best
                )

                if fit>best_fitness:
                    best_fitness=fit
                    best_overall=best


            fitness_history.append(
                best_fitness
            )

        return (
            best_overall,
            best_fitness,
            fitness_history
        )



    schedule,fitness,history=\
        run_island_genetic_algorithm()


    return {
        "schedule":schedule,
        "best_fitness":fitness,
        "fitness_history":history,
        "matches_count":len(matches),
        "teams_count":len(teams),
        "venues_count":len(venues)
    }

