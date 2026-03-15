using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class NPCSchedule : MonoBehaviour
    {
        [Header("Schedule Settings")]
        [SerializeField] private string m_npcName;
        [SerializeField] private NPCCategory m_category = NPCCategory.Civilian;

        [Header("Daily Schedule")]
        [SerializeField] private ScheduleEntry[] m_dailySchedule;

        private float m_currentTime;
        private int m_currentActivityIndex;
        private bool m_scheduleActive = true;
        private Transform m_currentLocation;
        private NPCScheduleManager m_manager;

        public enum NPCCategory
        {
            Civilian,
            Worker,
            Student,
            Shopkeeper,
            Emergency,
            Homeless
        }

        [System.Serializable]
        public class ScheduleEntry
        {
            public int startHour;
            public int endHour;
            public ScheduleActivity activity;
            public Transform location;
            public string description;
        }

        public enum ScheduleActivity
        {
            Sleep,
            WakeUp,
            Breakfast,
            Commute,
            Work,
            Lunch,
            Shopping,
            Socialize,
            Exercise,
            Dinner,
            Leisure,
            ReturnHome
        }

        private void Start()
        {
            FindScheduleManager();
            SetRandomSchedule();
        }

        private void FindScheduleManager()
        {
            m_manager = FindObjectOfType<NPCScheduleManager>();
        }

        private void SetRandomSchedule()
        {
            List<ScheduleEntry> schedule = new List<ScheduleEntry>();

            switch (m_category)
            {
                case NPCCategory.Worker:
                    schedule = CreateWorkerSchedule();
                    break;
                case NPCCategory.Student:
                    schedule = CreateStudentSchedule();
                    break;
                case NPCCategory.Shopkeeper:
                    schedule = CreateShopkeeperSchedule();
                    break;
                case NPCCategory.Civilian:
                default:
                    schedule = CreateCivilianSchedule();
                    break;
            }

            m_dailySchedule = schedule.ToArray();
        }

        private List<ScheduleEntry> CreateWorkerSchedule()
        {
            List<ScheduleEntry> schedule = new List<ScheduleEntry>();

            schedule.Add(new ScheduleEntry { startHour = 0, endHour = 6, activity = ScheduleActivity.Sleep, description = "Sleeping" });
            schedule.Add(new ScheduleEntry { startHour = 6, endHour = 7, activity = ScheduleActivity.WakeUp, description = "Waking up" });
            schedule.Add(new ScheduleEntry { startHour = 7, endHour = 8, activity = ScheduleActivity.Breakfast, description = "Breakfast" });
            schedule.Add(new ScheduleEntry { startHour = 8, endHour = 9, activity = ScheduleActivity.Commute, description = "Commuting to work" });
            schedule.Add(new ScheduleEntry { startHour = 9, endHour = 12, activity = ScheduleActivity.Work, description = "Working" });
            schedule.Add(new ScheduleEntry { startHour = 12, endHour = 13, activity = ScheduleActivity.Lunch, description = "Lunch break" });
            schedule.Add(new ScheduleEntry { startHour = 13, endHour = 17, activity = ScheduleActivity.Work, description = "Working" });
            schedule.Add(new ScheduleEntry { startHour = 17, endHour = 18, activity = ScheduleActivity.Commute, description = "Commuting home" });
            schedule.Add(new ScheduleEntry { startHour = 18, endHour = 19, activity = ScheduleActivity.Dinner, description = "Dinner" });
            schedule.Add(new ScheduleEntry { startHour = 19, endHour = 22, activity = ScheduleActivity.Leisure, description = "Free time" });
            schedule.Add(new ScheduleEntry { startHour = 22, endHour = 24, activity = ScheduleActivity.Sleep, description = "Sleeping" });

            return schedule;
        }

        private List<ScheduleEntry> CreateStudentSchedule()
        {
            List<ScheduleEntry> schedule = new List<ScheduleEntry>();

            schedule.Add(new ScheduleEntry { startHour = 0, endHour = 7, activity = ScheduleActivity.Sleep, description = "Sleeping" });
            schedule.Add(new ScheduleEntry { startHour = 7, endHour = 8, activity = ScheduleActivity.WakeUp, description = "Getting ready" });
            schedule.Add(new ScheduleEntry { startHour = 8, endHour = 14, activity = ScheduleActivity.Work, description = "School/Classes" });
            schedule.Add(new ScheduleEntry { startHour = 14, endHour = 15, activity = ScheduleActivity.Lunch, description = "Lunch" });
            schedule.Add(new ScheduleEntry { startHour = 15, endHour = 17, activity = ScheduleActivity.Exercise, description = "Sports/Activities" });
            schedule.Add(new ScheduleEntry { startHour = 17, endHour = 18, activity = ScheduleActivity.Commute, description = "Going home" });
            schedule.Add(new ScheduleEntry { startHour = 18, endHour = 19, activity = ScheduleActivity.Dinner, description = "Dinner" });
            schedule.Add(new ScheduleEntry { startHour = 19, endHour = 21, activity = ScheduleActivity.Leisure, description = "Homework/Relaxing" });
            schedule.Add(new ScheduleEntry { startHour = 21, endHour = 24, activity = ScheduleActivity.Sleep, description = "Sleeping" });

            return schedule;
        }

        private List<ScheduleEntry> CreateShopkeeperSchedule()
        {
            List<ScheduleEntry> schedule = new List<ScheduleEntry>();

            schedule.Add(new ScheduleEntry { startHour = 0, endHour = 6, activity = ScheduleActivity.Sleep, description = "Sleeping" });
            schedule.Add(new ScheduleEntry { startHour = 6, endHour = 7, activity = ScheduleActivity.WakeUp, description = "Waking up" });
            schedule.Add(new ScheduleEntry { startHour = 7, endHour = 8, activity = ScheduleActivity.Commute, description = "Opening shop" });
            schedule.Add(new ScheduleEntry { startHour = 8, endHour = 12, activity = ScheduleActivity.Work, description = "Running shop" });
            schedule.Add(new ScheduleEntry { startHour = 12, endHour = 13, activity = ScheduleActivity.Lunch, description = "Lunch break" });
            schedule.Add(new ScheduleEntry { startHour = 13, endHour = 18, activity = ScheduleActivity.Work, description = "Running shop" });
            schedule.Add(new ScheduleEntry { startHour = 18, endHour = 19, activity = ScheduleActivity.Commute, description = "Closing shop" });
            schedule.Add(new ScheduleEntry { startHour = 19, endHour = 21, activity = ScheduleActivity.Socialize, description = "Free time" });
            schedule.Add(new ScheduleEntry { startHour = 21, endHour = 24, activity = ScheduleActivity.Sleep, description = "Sleeping" });

            return schedule;
        }

        private List<ScheduleEntry> CreateCivilianSchedule()
        {
            List<ScheduleEntry> schedule = new List<ScheduleEntry>();

            schedule.Add(new ScheduleEntry { startHour = 0, endHour = 7, activity = ScheduleActivity.Sleep, description = "Sleeping" });
            schedule.Add(new ScheduleEntry { startHour = 7, endHour = 8, activity = ScheduleActivity.WakeUp, description = "Waking up" });
            schedule.Add(new ScheduleEntry { startHour = 8, endHour = 10, activity = ScheduleActivity.Breakfast, description = "Morning routine" });
            schedule.Add(new ScheduleEntry { startHour = 10, endHour = 12, activity = ScheduleActivity.Shopping, description = "Running errands" });
            schedule.Add(new ScheduleEntry { startHour = 12, endHour = 13, activity = ScheduleActivity.Lunch, description = "Lunch" });
            schedule.Add(new ScheduleEntry { startHour = 13, endHour = 16, activity = ScheduleActivity.Socialize, description = "Meeting friends" });
            schedule.Add(new ScheduleEntry { startHour = 16, endHour = 18, activity = ScheduleActivity.Exercise, description = "Exercise" });
            schedule.Add(new ScheduleEntry { startHour = 18, endHour = 19, activity = ScheduleActivity.Dinner, description = "Dinner" });
            schedule.Add(new ScheduleEntry { startHour = 19, endHour = 22, activity = ScheduleActivity.Leisure, description = "Watching TV/Gaming" });
            schedule.Add(new ScheduleEntry { startHour = 22, endHour = 24, activity = ScheduleActivity.Sleep, description = "Sleeping" });

            return schedule;
        }

        private void Update()
        {
            if (!m_scheduleActive || m_dailySchedule == null) return;

            UpdateSchedule();
        }

        private void UpdateSchedule()
        {
            var timeOfDay = FindObjectOfType<World.EnhancedTimeOfDay>();
            if (timeOfDay == null) return;

            m_currentTime = timeOfDay.CurrentTime;
            int currentHour = Mathf.FloorToInt(m_currentTime);

            ScheduleEntry currentActivity = GetCurrentActivity(currentHour);

            if (currentActivity != null)
            {
                if (m_currentActivityIndex != currentHour)
                {
                    OnActivityChange(currentActivity);
                }
            }
        }

        private ScheduleEntry GetCurrentActivity(int hour)
        {
            if (m_dailySchedule == null) return null;

            foreach (var entry in m_dailySchedule)
            {
                if (hour >= entry.startHour && hour < entry.endHour)
                {
                    return entry;
                }
            }

            return null;
        }

        private void OnActivityChange(ScheduleEntry entry)
        {
            Debug.Log($"[NPCSchedule] {m_npcName}: {entry.description}");

            if (m_manager != null)
            {
                m_manager.OnNPCActivityChanged(this, entry);
            }
        }

        public ScheduleActivity GetCurrentActivity()
        {
            int currentHour = Mathf.FloorToInt(m_currentTime);
            ScheduleEntry entry = GetCurrentActivity(currentHour);
            return entry?.activity ?? ScheduleActivity.Leisure;
        }

        public void SetLocation(Transform location)
        {
            m_currentLocation = location;
        }

        public Transform GetCurrentLocation() => m_currentLocation;
        public NPCCategory GetCategory() => m_category;
    }

    public class NPCScheduleManager : MonoBehaviour
    {
        public static NPCScheduleManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enableSchedules = true;
        [SerializeField] private int m_maxScheduledNPCs = 50;

        [Header("Locations")]
        [SerializeField] private Transform[] m_homeLocations;
        [SerializeField] private Transform[] m_workLocations;
        [SerializeField] private Transform[] m_shopLocations;
        [SerializeField] private Transform[] m_parkLocations;

        private List<NPCSchedule> m_scheduledNPCs = new List<NPCSchedule>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (!m_enableSchedules) return;
        }

        public void RegisterNPC(NPCSchedule npc)
        {
            if (!m_scheduledNPCs.Contains(npc))
            {
                m_scheduledNPCs.Add(npc);
            }
        }

        public void OnNPCActivityChanged(NPCSchedule npc, NPCSchedule.ScheduleEntry entry)
        {
            switch (entry.activity)
            {
                case NPCSchedule.ScheduleActivity.Work:
                    npc.SetLocation(GetRandomLocation(m_workLocations));
                    break;
                case NPCSchedule.ScheduleActivity.Shopping:
                case NPCSchedule.ScheduleActivity.Socialize:
                    npc.SetLocation(GetRandomLocation(m_shopLocations));
                    break;
                case NPCSchedule.ScheduleActivity.Exercise:
                    npc.SetLocation(GetRandomLocation(m_parkLocations));
                    break;
                case NPCSchedule.ScheduleActivity.Sleep:
                case NPCSchedule.ScheduleActivity.WakeUp:
                case NPCSchedule.ScheduleActivity.Breakfast:
                case NPCSchedule.ScheduleActivity.Dinner:
                case NPCSchedule.ScheduleActivity.Leisure:
                    npc.SetLocation(GetRandomLocation(m_homeLocations));
                    break;
            }
        }

        private Transform GetRandomLocation(Transform[] locations)
        {
            if (locations == null || locations.Length == 0) return null;
            return locations[Random.Range(0, locations.Length)];
        }

        public int GetScheduledCount() => m_scheduledNPCs.Count;
    }
}
