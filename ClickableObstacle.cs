using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// =====================================================================
// RARITY ENUM
// =====================================================================

public enum EffectRarity { Basic, Rare, Legendary }


// =====================================================================
// CLICKABLE OBSTACLE – 21 efekat, 3 rariteta
// =====================================================================

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ClickableObstacle : MonoBehaviour, IPointerDownHandler
{
    // =================================================================
    //  FIRST CLICK – Falling
    // =================================================================

    [Header("FIRST CLICK – Falling")]
    [SerializeField] private float minFallGravity = 2.5f;
    [SerializeField] private float maxFallGravity = 4.5f;
    [SerializeField] private float minFirstSpin = 80f;
    [SerializeField] private float maxFirstSpin = 220f;
    [SerializeField] private float minVerticalKick = -1.2f;
    [SerializeField] private float maxVerticalKick = 0.2f;
    [SerializeField] private bool keepHorizontalMomentum = true;

    // =================================================================
    //  EXTRA CLICKS
    // =================================================================

    [Header("EXTRA CLICKS")]
    [SerializeField] private int maxExtraClicks = 8;
    [SerializeField] private float extraClickCooldown = 0.12f;

    // =================================================================
    //  RARITY WEIGHTS
    // =================================================================

    [Header("Rarity Weights")]
    [Tooltip("Šansa za Basic efekat (default 60)")]
    [SerializeField] private float basicWeight = 60f;
    [Tooltip("Šansa za Rare efekat (default 30)")]
    [SerializeField] private float rareWeight = 30f;
    [Tooltip("Šansa za Legendary efekat (default 10)")]
    [SerializeField] private float legendaryWeight = 10f;

    // =================================================================
    //  HIT JUICE (svaki klik)
    // =================================================================

    [Header("Hit Juice (every click)")]
    [SerializeField] private float scalePunchAmount = 1.25f;
    [SerializeField] private float scalePunchDuration = 0.09f;
    [SerializeField]
    private Color[] flashPalette = new Color[]
    {
        new Color(1f, 0.85f, 0.3f, 1f),   // topla žuta
        new Color(1f, 0.6f, 0.2f, 1f),    // narandžasta
        new Color(1f, 0.45f, 0.35f, 1f),  // topla crvena
    };
    [SerializeField] private float hitFlashDuration = 0.07f;
    [SerializeField] private float impactBurstScale = 0.6f;
    [SerializeField] private float impactBurstDuration = 0.15f;

    // =================================================================
    //  AUDIO
    // =================================================================

    [Header("Audio (optional)")]
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioClip[] legendarySounds;
    [SerializeField] private float minPitch = 0.88f;
    [SerializeField] private float maxPitch = 1.12f;
    [SerializeField][Range(0f, 1f)] private float hitVolume = 0.7f;

    // =================================================================
    //  SIDE KICK (Basic)
    // =================================================================

    [Header("Side Kick")]
    [SerializeField] private float minSideKick = 4f;
    [SerializeField] private float maxSideKick = 8f;
    [SerializeField] private float sideKickUpwardForce = 2f;

    // =================================================================
    //  POP UP (Basic)
    // =================================================================

    [Header("Pop Up")]
    [SerializeField] private float minPopForce = 5f;
    [SerializeField] private float maxPopForce = 9f;

    // =================================================================
    //  SLAM DOWN (Basic)
    // =================================================================

    [Header("Slam Down")]
    [SerializeField] private float minSlamForce = 6f;
    [SerializeField] private float maxSlamForce = 11f;

    // =================================================================
    //  SPIN BURST (Basic)
    // =================================================================

    [Header("Spin Burst")]
    [SerializeField] private float minExtraSpin = 250f;
    [SerializeField] private float maxExtraSpin = 600f;

    // =================================================================
    //  HITSTOP (Basic)
    // =================================================================

    [Header("Hitstop Hit")]
    [SerializeField] private float hitstopFreezeDuration = 0.065f;
    [SerializeField] private float hitstopForceMin = 4f;
    [SerializeField] private float hitstopForceMax = 9f;

    // =================================================================
    //  EXPLODE (Rare)
    // =================================================================

    [Header("Explode (Rare)")]
    [SerializeField] private int debrisCount = 8;
    [SerializeField] private float debrisSpeed = 8f;
    [SerializeField] private float debrisLifetime = 1.2f;
    [SerializeField] private float debrisSize = 0.15f;

    // =================================================================
    //  CLONE SPLIT (Rare)
    // =================================================================

    [Header("Clone Split (Rare)")]
    [SerializeField] private int cloneCount = 3;
    [SerializeField] private float cloneSplitForce = 6f;
    [SerializeField] private float cloneLifetime = 2f;
    [SerializeField] private float cloneScaleMultiplier = 0.6f;

    // =================================================================
    //  BOOMERANG (Rare)
    // =================================================================

    [Header("Boomerang (Rare)")]
    [SerializeField] private float boomerangOutForce = 10f;
    [SerializeField] private float boomerangReturnDelay = 0.4f;
    [SerializeField] private float boomerangReturnForce = 14f;
    [SerializeField] private float boomerangLifetime = 1.5f;

    // =================================================================
    //  ZIGZAG FALL (Rare)
    // =================================================================

    [Header("Zigzag Fall (Rare)")]
    [SerializeField] private float zigzagAmplitude = 4f;
    [SerializeField] private float zigzagFrequency = 8f;
    [SerializeField] private float zigzagDuration = 1.5f;
    [SerializeField] private float zigzagDownForce = 6f;

    // =================================================================
    //  PAPER FOLD (Rare)
    // =================================================================

    [Header("Paper Fold (Rare)")]
    [SerializeField] private float paperFoldDuration = 1.8f;
    [SerializeField] private float paperFoldFrequency = 6f;
    [SerializeField] private float paperFoldFallGravity = 0.4f;

    // =================================================================
    //  GHOST TRAIL (Rare)
    // =================================================================

    [Header("Ghost Trail (Rare)")]
    [SerializeField] private float ghostTrailDuration = 1.2f;
    [SerializeField] private float ghostTrailInterval = 0.04f;
    [SerializeField] private float ghostFadeTime = 0.35f;

    // =================================================================
    //  PUSH INTO DISTANCE (Rare)
    // =================================================================

    [Header("Push Into Distance (Rare)")]
    [SerializeField] private float distanceEffectDuration = 0.65f;
    [SerializeField] private float distanceFinalScale = 0.08f;
    [SerializeField] private float distancePushForce = 5f;

    // =================================================================
    //  COLOR CYCLE RAINBOW (Rare)
    // =================================================================

    [Header("Color Cycle Rainbow (Rare)")]
    [SerializeField] private float rainbowDuration = 1.5f;
    [SerializeField] private float rainbowCycleSpeed = 4f;
    [SerializeField] private float rainbowKickForce = 6f;

    // =================================================================
    //  INFLATE & POP (Legendary)
    // =================================================================

    [Header("Inflate & Pop (Legendary)")]
    [SerializeField] private float inflateDuration = 0.35f;
    [SerializeField] private float inflateMaxScale = 2.5f;

    // =================================================================
    //  TELEPORT FLASH (Legendary)
    // =================================================================

    [Header("Teleport Flash (Legendary)")]
    [SerializeField] private int teleportJumps = 4;
    [SerializeField] private float teleportInterval = 0.13f;
    [SerializeField] private float teleportRange = 3f;

    // =================================================================
    //  SLOW-MO HIT (Legendary)
    // =================================================================

    [Header("Slow-Mo Hit (Legendary)")]
    [SerializeField] private float slowMoTimeScale = 0.15f;
    [SerializeField] private float slowMoDuration = 0.5f;
    [SerializeField] private float slowMoReturnSpeed = 3f;

    // =================================================================
    //  FREEZE & SHATTER (Legendary)
    // =================================================================

    [Header("Freeze & Shatter (Legendary)")]
    [SerializeField] private float freezeDuration = 0.5f;
    [SerializeField] private Color freezeTint = new Color(0.5f, 0.8f, 1f, 1f);
    [SerializeField] private int shatterPieceCount = 12;

    // =================================================================
    //  REWIND (Legendary)
    // =================================================================

    [Header("Rewind (Legendary)")]
    [SerializeField] private float rewindPlaybackSpeed = 0.012f;
    [SerializeField] private Color rewindTint = new Color(0.3f, 1f, 0.9f, 0.8f);

    // =================================================================
    //  MAGNET PULL (Legendary)
    // =================================================================

    [Header("Magnet Pull (Legendary)")]
    [SerializeField] private float magnetPullDuration = 0.3f;
    [SerializeField] private float magnetPullSpeed = 15f;
    [SerializeField] private float magnetLaunchForce = 14f;

    // =================================================================
    //  CHAIN REACTION (Legendary)
    // =================================================================

    [Header("Chain Reaction (Legendary)")]
    [SerializeField] private float chainRadius = 5f;
    [SerializeField] private float chainForce = 8f;
    [SerializeField] private float chainSelfForce = 10f;

    // =================================================================
    //  SHRINK SPIRAL (Legendary)
    // =================================================================

    [Header("Shrink Spiral (Legendary)")]
    [SerializeField] private float spiralDuration = 1.2f;
    [SerializeField] private float spiralSpeed = 720f;
    [SerializeField] private float spiralRadius = 1.5f;

    // =================================================================
    //  SCREEN SHAKE (per-effect, not global)
    // =================================================================

    [Header("Screen Shake (per-effect)")]
    [SerializeField] private float shakeIntensity = 0.15f;
    [SerializeField] private float shakeDuration = 0.2f;

    // =================================================================
    //  GROUND COLLISION JUICE
    // =================================================================

    [Header("Ground Collision Juice")]
    [SerializeField] private bool enableSquashStretch = true;
    [SerializeField] private float squashAmount = 0.75f;
    [SerializeField] private float squashDuration = 0.15f;
    [SerializeField] private bool enableDustPuff = true;
    [SerializeField] private int dustParticleCount = 6;
    [SerializeField] private float dustSpeed = 2f;
    [SerializeField] private float dustLifetime = 0.5f;

    // =================================================================
    //  TRAIL
    // =================================================================

    [Header("Trail")]
    [SerializeField] private bool enableHitTrail = true;
    [SerializeField] private float trailDuration = 0.3f;
    [SerializeField] private Color trailStartColor = new Color(1f, 0.6f, 0f, 0.8f);
    [SerializeField] private Color trailEndColor = new Color(1f, 0.2f, 0f, 0f);


    // =================================================================
    //  PRIVATE STATE
    // =================================================================

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private ObstacleMovementBase movement;
    private TrailRenderer trail;
    private AudioSource audioSource;

    private bool falling = false;
    private bool interactionLocked = false;

    private int extraClicks = 0;
    private float lastExtraClickTime = -100f;

    private Vector3 originalScale;
    private Color originalColor;

    private Coroutine scalePunchCoroutine;
    private Coroutine colorFlashCoroutine;
    private Coroutine squashCoroutine;

    // Rewind position buffer
    private List<Vector3> rewindPositions = new List<Vector3>();
    private List<float> rewindRotations = new List<float>();
    private const int MAX_REWIND_FRAMES = 120;
    private bool rewindActive = false;

    // Impact burst sprite (cached)
    private Sprite cachedCircleSprite;

    // Effect counts
    private const int BASIC_COUNT = 5;
    private const int RARE_COUNT = 8;
    private const int LEGENDARY_COUNT = 8;


    // =================================================================
    //  AWAKE
    // =================================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        movement = GetComponent<ObstacleMovementBase>();

        originalScale = transform.localScale;
        originalColor = spriteRenderer != null
            ? spriteRenderer.color
            : Color.white;

        SetupTrail();
        SetupAudio();
        CacheCircleSprite();
    }


    private void SetupTrail()
    {
        if (!enableHitTrail) return;

        trail = GetComponent<TrailRenderer>();

        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
            trail.time = trailDuration;
            trail.startWidth = 0.2f;
            trail.endWidth = 0.02f;
            trail.startColor = trailStartColor;
            trail.endColor = trailEndColor;
            trail.material = new Material(Shader.Find("Sprites/Default"));

            trail.sortingOrder = spriteRenderer != null
                ? spriteRenderer.sortingOrder - 1
                : 0;

            trail.enabled = false;
        }
    }


    private void SetupAudio()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }


    private void CacheCircleSprite()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float center = size * 0.5f;
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(
                    new Vector2(x, y),
                    new Vector2(center, center));

                float alpha = Mathf.Clamp01(1f - dist / radius);
                alpha *= alpha; // Soft edge
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();
        cachedCircleSprite = Sprite.Create(
            tex,
            new Rect(0, 0, size, size),
            Vector2.one * 0.5f,
            size);
    }


    // =================================================================
    //  FIXED UPDATE – snima pozicije za Rewind
    // =================================================================

    private void FixedUpdate()
    {
        if (!falling || rewindActive) return;

        rewindPositions.Add(transform.position);
        rewindRotations.Add(transform.eulerAngles.z);

        if (rewindPositions.Count > MAX_REWIND_FRAMES)
        {
            rewindPositions.RemoveAt(0);
            rewindRotations.RemoveAt(0);
        }
    }


    // =================================================================
    //  CLICK HANDLER
    // =================================================================

    public void OnPointerDown(PointerEventData eventData)
    {
        if (interactionLocked) return;

        Vector3 clickWorld = GetClickWorldPos(eventData);

        // ─── PRVI KLIK ───
        if (!falling)
        {
            StartFalling();
            DoScalePunch();
            DoColorFlash();
            SpawnImpactBurst(clickWorld);
            PlayHitSound(false);
            return;
        }

        // ─── SLEDEĆI KLIKOVI ───
        if (Time.unscaledTime - lastExtraClickTime < extraClickCooldown)
            return;

        if (extraClicks >= maxExtraClicks)
            return;

        lastExtraClickTime = Time.unscaledTime;
        extraClicks++;

        // Juice na svaki klik
        DoScalePunch();
        DoColorFlash();
        SpawnImpactBurst(clickWorld);
        PlayHitSound(false);

        TriggerRandomExtraEffect(eventData);
    }


    private Vector3 GetClickWorldPos(PointerEventData eventData)
    {
        Camera cam = eventData.pressEventCamera;
        if (cam == null) cam = Camera.main;
        if (cam == null) return transform.position;

        Vector3 world = cam.ScreenToWorldPoint(eventData.position);
        world.z = transform.position.z;
        return world;
    }


    // =================================================================
    //  PRVI KLIK
    // =================================================================

    public void StartFalling()
    {
        if (falling) return;
        falling = true;

        Vector2 releaseVelocity = Vector2.zero;

        if (movement != null)
        {
            if (keepHorizontalMomentum)
                releaseVelocity = movement.GetReleaseVelocity();

            movement.StopMovement();
        }

        rb.constraints = RigidbodyConstraints2D.None;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;

        rb.gravityScale = Random.Range(minFallGravity, maxFallGravity);

        releaseVelocity.y = Random.Range(minVerticalKick, maxVerticalKick);

        rb.linearVelocity = keepHorizontalMomentum
            ? releaseVelocity
            : new Vector2(0f, releaseVelocity.y);

        rb.angularVelocity = Random.Range(minFirstSpin, maxFirstSpin)
            * RandomSign();

        if (trail != null) trail.enabled = true;

        Debug.Log("[CLICK] FIRST CLICK -> FALL | " + gameObject.name);
    }


    // =================================================================
    //  RARITY PICKER + EFFECT DISPATCH
    // =================================================================

    private EffectRarity PickRarity()
    {
        float total = basicWeight + rareWeight + legendaryWeight;
        float roll = Random.Range(0f, total);

        if (roll < basicWeight) return EffectRarity.Basic;
        if (roll < basicWeight + rareWeight) return EffectRarity.Rare;
        return EffectRarity.Legendary;
    }


    private void TriggerRandomExtraEffect(PointerEventData eventData)
    {
        EffectRarity rarity = PickRarity();

        switch (rarity)
        {
            case EffectRarity.Basic:
                TriggerBasicEffect(eventData);
                break;

            case EffectRarity.Rare:
                TriggerRareEffect(eventData);
                break;

            case EffectRarity.Legendary:
                // Legendary indikator – zlatni flash pre efekta
                DoLegendaryFlash();
                TriggerLegendaryEffect(eventData);
                break;
        }
    }


    private void TriggerBasicEffect(PointerEventData eventData)
    {
        int effect = Random.Range(0, BASIC_COUNT);

        switch (effect)
        {
            case 0: SideKick(eventData); break;
            case 1: PopUp(); break;
            case 2: SlamDown(); break;
            case 3: SpinBurst(); break;
            case 4: HitstopHit(); break;
        }
    }


    private void TriggerRareEffect(PointerEventData eventData)
    {
        int effect = Random.Range(0, RARE_COUNT);

        switch (effect)
        {
            case 0: Explode(); break;
            case 1: CloneSplit(); break;
            case 2: BoomerangEffect(); break;
            case 3: ZigzagFall(); break;
            case 4: PaperFold(); break;
            case 5: GhostTrailBurst(); break;
            case 6: PushIntoDistance(); break;
            case 7: ColorCycleRainbow(); break;
        }
    }


    private void TriggerLegendaryEffect(PointerEventData eventData)
    {
        int effect = Random.Range(0, LEGENDARY_COUNT);

        switch (effect)
        {
            case 0: InflateAndPop(); break;
            case 1: TeleportFlash(); break;
            case 2: SlowMoHit(); break;
            case 3: FreezeAndShatter(); break;
            case 4: RewindEffect(); break;
            case 5: MagnetPull(eventData); break;
            case 6: ChainReaction(); break;
            case 7: ShrinkSpiral(); break;
        }
    }


    // =================================================================
    //  ★ BASIC EFEKTI (5)
    // =================================================================

    #region Basic Effects

    // ─── 1. SIDE KICK ───────────────────────────────────────────────

    private void SideKick(PointerEventData eventData)
    {
        Camera cam = eventData.pressEventCamera;
        if (cam == null) cam = Camera.main;

        float direction;

        if (cam != null)
        {
            Vector3 clickWorld = cam.ScreenToWorldPoint(eventData.position);
            direction = clickWorld.x < transform.position.x ? 1f : -1f;
        }
        else
        {
            direction = RandomSign();
        }

        // Varijabilni otpor – veći scale = manja sila
        float scaleFactor = Mathf.Clamp(
            1f / transform.localScale.magnitude, 0.3f, 2f);

        Vector2 force = new Vector2(
            direction * Random.Range(minSideKick, maxSideKick) * scaleFactor,
            sideKickUpwardForce);

        rb.AddForce(force, ForceMode2D.Impulse);
        AddRandomSpin();

        Debug.Log("[CLICK] BASIC -> SIDE KICK");
    }


    // ─── 2. POP UP ──────────────────────────────────────────────────

    private void PopUp()
    {
        float force = Random.Range(minPopForce, maxPopForce);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        AddRandomSpin();

        Debug.Log("[CLICK] BASIC -> POP UP");
    }


    // ─── 3. SLAM DOWN ───────────────────────────────────────────────

    private void SlamDown()
    {
        float force = Random.Range(minSlamForce, maxSlamForce);

        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        rb.AddForce(Vector2.down * force, ForceMode2D.Impulse);
        AddRandomSpin();

        Debug.Log("[CLICK] BASIC -> SLAM DOWN");
    }


    // ─── 4. SPIN BURST ──────────────────────────────────────────────

    private void SpinBurst()
    {
        rb.angularVelocity =
            Random.Range(minExtraSpin, maxExtraSpin) * RandomSign();

        Vector2 kick = new Vector2(
            Random.Range(-2f, 2f),
            Random.Range(1f, 4f));

        rb.AddForce(kick, ForceMode2D.Impulse);

        Debug.Log("[CLICK] BASIC -> SPIN BURST");
    }


    // ─── 5. HITSTOP HIT ─────────────────────────────────────────────

    private void HitstopHit()
    {
        Vector2 force = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f)).normalized
            * Random.Range(hitstopForceMin, hitstopForceMax);

        StartCoroutine(HitstopRoutine(force));

        Debug.Log("[CLICK] BASIC -> HITSTOP HIT");
    }


    private IEnumerator HitstopRoutine(Vector2 force)
    {
        // Sačuvaj stanje
        Vector2 savedVel = rb.linearVelocity;
        float savedAngular = rb.angularVelocity;
        float savedGravity = rb.gravityScale;

        // Zamrzni objekat
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;

        // Smanji scale na trenutak za "impact" osećaj
        Vector3 frozenScale = transform.localScale * 0.9f;
        Vector3 preScale = transform.localScale;
        transform.localScale = frozenScale;

        yield return new WaitForSecondsRealtime(hitstopFreezeDuration);

        // Vrati i primeni silu
        transform.localScale = preScale;
        rb.gravityScale = savedGravity;
        rb.linearVelocity = savedVel;
        rb.angularVelocity = savedAngular;

        rb.AddForce(force, ForceMode2D.Impulse);
        AddRandomSpin();
    }

    #endregion


    // =================================================================
    //  ◆ RARE EFEKTI (8)
    // =================================================================

    #region Rare Effects

    // ─── 6. EXPLODE ─────────────────────────────────────────────────

    private void Explode()
    {
        if (interactionLocked) return;
        interactionLocked = true;

        TriggerScreenShake(shakeIntensity * 1.5f, shakeDuration);

        for (int i = 0; i < debrisCount; i++)
            SpawnDebris(transform.position);

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        col.enabled = false;
        StartCoroutine(DelayedDestroy(0.05f));

        Debug.Log("[CLICK] ◆ RARE ◆ -> EXPLODE");
    }


    // ─── 7. CLONE SPLIT ─────────────────────────────────────────────

    private void CloneSplit()
    {
        if (interactionLocked) return;
        interactionLocked = true;

        for (int i = 0; i < cloneCount; i++)
            SpawnClone();

        col.enabled = false;
        StartCoroutine(DelayedDestroy(0.05f));

        Debug.Log("[CLICK] ◆ RARE ◆ -> CLONE SPLIT");
    }


    private void SpawnClone()
    {
        GameObject clone = Instantiate(gameObject);
        clone.name = gameObject.name + "_Clone";

        ClickableObstacle cs = clone.GetComponent<ClickableObstacle>();
        if (cs != null) Destroy(cs);

        ObstacleMovementBase cm = clone.GetComponent<ObstacleMovementBase>();
        if (cm != null) Destroy(cm);

        clone.transform.position = transform.position;
        clone.transform.localScale = originalScale * cloneScaleMultiplier;

        Rigidbody2D cloneRB = clone.GetComponent<Rigidbody2D>();
        if (cloneRB != null)
        {
            cloneRB.constraints = RigidbodyConstraints2D.None;
            cloneRB.bodyType = RigidbodyType2D.Dynamic;
            cloneRB.simulated = true;
            cloneRB.gravityScale = Random.Range(minFallGravity, maxFallGravity);
            cloneRB.linearVelocity = Random.insideUnitCircle.normalized * cloneSplitForce;
            cloneRB.angularVelocity = Random.Range(-400f, 400f);
        }

        SpriteRenderer cloneSR = clone.GetComponentInChildren<SpriteRenderer>();
        if (cloneSR != null)
        {
            DebrisFader fader = clone.AddComponent<DebrisFader>();
            fader.Init(cloneLifetime, cloneSR);
        }

        Destroy(clone, cloneLifetime + 0.1f);
    }


    // ─── 8. BOOMERANG ───────────────────────────────────────────────

    private void BoomerangEffect()
    {
        float dir = RandomSign();
        rb.linearVelocity = Vector2.zero;

        Vector2 outForce = new Vector2(
            dir * boomerangOutForce,
            Random.Range(2f, 5f));

        rb.AddForce(outForce, ForceMode2D.Impulse);
        AddRandomSpin();

        StartCoroutine(BoomerangReturn(dir));

        Debug.Log("[CLICK] ◆ RARE ◆ -> BOOMERANG");
    }


    private IEnumerator BoomerangReturn(float originalDir)
    {
        yield return new WaitForSeconds(boomerangReturnDelay);
        if (rb == null) yield break;

        Vector2 returnForce = new Vector2(
            -originalDir * boomerangReturnForce,
            Random.Range(1f, 3f));

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(returnForce, ForceMode2D.Impulse);
        rb.angularVelocity = -rb.angularVelocity * 1.5f;

        yield return new WaitForSeconds(boomerangLifetime);

        if (gameObject != null)
            StartCoroutine(QuickFadeDestroy(0.3f));
    }


    // ─── 9. ZIGZAG FALL ─────────────────────────────────────────────

    private void ZigzagFall()
    {
        rb.gravityScale = 0.5f;
        rb.AddForce(Vector2.down * zigzagDownForce, ForceMode2D.Impulse);

        StartCoroutine(ZigzagRoutine());

        Debug.Log("[CLICK] ◆ RARE ◆ -> ZIGZAG FALL");
    }


    private IEnumerator ZigzagRoutine()
    {
        float elapsed = 0f;

        while (elapsed < zigzagDuration)
        {
            elapsed += Time.deltaTime;

            float lateralForce =
                Mathf.Sin(elapsed * zigzagFrequency) * zigzagAmplitude;

            rb.AddForce(new Vector2(lateralForce, 0f), ForceMode2D.Force);

            float targetAngle = Mathf.Sin(elapsed * zigzagFrequency) * 30f;
            rb.angularVelocity = (targetAngle - transform.eulerAngles.z) * 5f;

            yield return null;
        }

        rb.gravityScale = Random.Range(minFallGravity, maxFallGravity);
    }


    // ─── 10. PAPER FOLD ─────────────────────────────────────────────

    private void PaperFold()
    {
        rb.gravityScale = paperFoldFallGravity;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.3f, -1f);

        StartCoroutine(PaperFoldRoutine());

        Debug.Log("[CLICK] ◆ RARE ◆ -> PAPER FOLD");
    }


    private IEnumerator PaperFoldRoutine()
    {
        float elapsed = 0f;
        Vector3 baseScale = transform.localScale;

        while (elapsed < paperFoldDuration)
        {
            elapsed += Time.deltaTime;

            // Scale X osciluje ka 0 i nazad — izgleda kao savijanje
            float fold = Mathf.Cos(elapsed * paperFoldFrequency * Mathf.PI);

            transform.localScale = new Vector3(
                baseScale.x * Mathf.Abs(fold) * 0.8f + baseScale.x * 0.2f,
                baseScale.y,
                baseScale.z);

            // Lateralno pomeranje sinhronizovano sa savijanjem
            rb.AddForce(
                new Vector2(fold * 2f, 0f),
                ForceMode2D.Force);

            yield return null;
        }

        transform.localScale = baseScale;
        rb.gravityScale = Random.Range(minFallGravity, maxFallGravity);
    }


    // ─── 11. GHOST TRAIL BURST ──────────────────────────────────────

    private void GhostTrailBurst()
    {
        // Jak kick + ghost kopije
        Vector2 kick = new Vector2(
            Random.Range(-5f, 5f),
            Random.Range(3f, 7f));

        rb.AddForce(kick, ForceMode2D.Impulse);
        AddRandomSpin();

        StartCoroutine(GhostTrailRoutine());

        Debug.Log("[CLICK] ◆ RARE ◆ -> GHOST TRAIL");
    }


    private IEnumerator GhostTrailRoutine()
    {
        float elapsed = 0f;

        while (elapsed < ghostTrailDuration)
        {
            SpawnGhostSprite();
            elapsed += ghostTrailInterval;
            yield return new WaitForSeconds(ghostTrailInterval);
        }
    }


    private void SpawnGhostSprite()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        GameObject ghost = new GameObject("Ghost");
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer ghostSR = ghost.AddComponent<SpriteRenderer>();
        ghostSR.sprite = spriteRenderer.sprite;

        Color ghostColor = originalColor;
        ghostColor.a = 0.5f;
        ghostSR.color = ghostColor;
        ghostSR.sortingOrder = spriteRenderer.sortingOrder - 1;

        GhostFader fader = ghost.AddComponent<GhostFader>();
        fader.Init(ghostFadeTime);

        Destroy(ghost, ghostFadeTime + 0.05f);
    }


    // ─── 12. PUSH INTO DISTANCE ─────────────────────────────────────

    private void PushIntoDistance()
    {
        if (interactionLocked) return;
        interactionLocked = true;

        Vector2 push = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-0.3f, 1f)).normalized * distancePushForce;

        rb.AddForce(push, ForceMode2D.Impulse);
        rb.angularVelocity = Random.Range(minExtraSpin, maxExtraSpin) * RandomSign();

        col.enabled = false;
        StartCoroutine(DistanceEffect());

        Debug.Log("[CLICK] ◆ RARE ◆ -> INTO DISTANCE");
    }


    private IEnumerator DistanceEffect()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = originalScale * distanceFinalScale;

        Color startColor = spriteRenderer != null
            ? spriteRenderer.color
            : Color.white;

        while (elapsed < distanceEffectDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / distanceEffectDuration);
            float smoothT = t * t * (3f - 2f * t);

            transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);

            if (spriteRenderer != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(startColor.a, 0f, smoothT);
                spriteRenderer.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }


    // ─── 13. COLOR CYCLE RAINBOW ────────────────────────────────────

    private void ColorCycleRainbow()
    {
        Vector2 kick = new Vector2(
            Random.Range(-3f, 3f),
            Random.Range(2f, 6f)).normalized * rainbowKickForce;

        rb.AddForce(kick, ForceMode2D.Impulse);
        AddRandomSpin();

        StartCoroutine(RainbowRoutine());

        Debug.Log("[CLICK] ◆ RARE ◆ -> COLOR CYCLE RAINBOW");
    }


    private IEnumerator RainbowRoutine()
    {
        if (spriteRenderer == null) yield break;

        float elapsed = 0f;

        while (elapsed < rainbowDuration)
        {
            elapsed += Time.deltaTime;

            float hue = Mathf.Repeat(elapsed * rainbowCycleSpeed, 1f);
            Color rainbow = Color.HSVToRGB(hue, 0.8f, 1f);

            spriteRenderer.color = rainbow;

            yield return null;
        }

        spriteRenderer.color = originalColor;
    }

    #endregion


    // =================================================================
    //  ★ LEGENDARY EFEKTI (8)
    // =================================================================

    #region Legendary Effects

    // ─── 14. INFLATE & POP ──────────────────────────────────────────

    private void InflateAndPop()
    {
        if (interactionLocked) return;
        interactionLocked = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;

        StartCoroutine(InflatePopRoutine());

        Debug.Log("[CLICK] ★ LEGENDARY ★ -> INFLATE & POP");
    }


    private IEnumerator InflatePopRoutine()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 maxScale = originalScale * inflateMaxScale;

        while (elapsed < inflateDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / inflateDuration);

            // Elastični wobble
            float wobble = 1f - Mathf.Cos(t * Mathf.PI * 4f)
                * (1f - t) * 0.15f;

            transform.localScale = Vector3.Lerp(startScale, maxScale, t) * wobble;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(
                    originalColor,
                    new Color(1f, 0.3f, 0.2f, 1f), t);
            }

            yield return null;
        }

        // POP!
        TriggerScreenShake(shakeIntensity * 2.5f, shakeDuration * 1.5f);
        PlayHitSound(true);

        for (int i = 0; i < debrisCount + 4; i++)
            SpawnDebris(transform.position);

        Destroy(gameObject);
    }


    // ─── 15. TELEPORT FLASH ─────────────────────────────────────────

    private void TeleportFlash()
    {
        if (interactionLocked) return;
        interactionLocked = true;

        StartCoroutine(TeleportRoutine());

        Debug.Log("[CLICK] ★ LEGENDARY ★ -> TELEPORT FLASH");
    }


    private IEnumerator TeleportRoutine()
    {
        Vector3 startPos = transform.position;
        if (trail != null) trail.enabled = false;

        for (int i = 0; i < teleportJumps; i++)
        {
            // Ghost na staroj poziciji
            SpawnGhostSprite();

            // Flash OUT
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.2f);

            yield return new WaitForSecondsRealtime(teleportInterval * 0.3f);

            // Teleport
            Vector2 offset = Random.insideUnitCircle * teleportRange;
            transform.position = startPos + new Vector3(offset.x, offset.y, 0f);

            // Impact burst na novoj poziciji
            SpawnImpactBurst(transform.position);

            // Flash IN
            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;

            yield return new WaitForSecondsRealtime(teleportInterval * 0.7f);

            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }

        if (trail != null) trail.enabled = true;

        Vector2 finalKick = new Vector2(
            Random.Range(-4f, 4f),
            Random.Range(-2f, 4f));

        rb.AddForce(finalKick, ForceMode2D.Impulse);
        AddRandomSpin();

        interactionLocked = false;
    }


    // ─── 16. SLOW-MO HIT ───────────────────────────────────────────

    private void SlowMoHit()
    {
        Vector2 kick = new Vector2(
            Random.Range(-5f, 5f),
            Random.Range(3f, 8f));

        rb.AddForce(kick, ForceMode2D.Impulse);
        AddRandomSpin();

        TriggerScreenShake(shakeIntensity * 1.5f, shakeDuration);

        StartCoroutine(SlowMotionRoutine());

        Debug.Log("[CLICK] ★ LEGENDARY ★ -> SLOW-MO HIT");
    }


    private IEnumerator SlowMotionRoutine()
    {
        Time.timeScale = slowMoTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(slowMoDuration);

        while (Time.timeScale < 0.99f)
        {
            Time.timeScale = Mathf.MoveTowards(
                Time.timeScale, 1f,
                Time.unscaledDeltaTime * slowMoReturnSpeed);

            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }


    // ─── 17. FREEZE & SHATTER ───────────────────────────────────────

    private void FreezeAndShatter()
    {
        if (interactionLocked) return;
        interactionLocked = true;

        StartCoroutine(FreezeShatterRoutine());

        Debug.Log("[CLICK] ★ LEGENDARY ★ -> FREEZE & SHATTER");
    }


    private IEnumerator FreezeShatterRoutine()
    {
        // Faza 1: ZAMRZNI
        Vector2 savedVel = rb.linearVelocity;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
        rb.simulated = false;

        // Plavi tint
        if (spriteRenderer != null)
            spriteRenderer.color = freezeTint;

        // Lagano drhtanje dok je zamrznut
        Vector3 frozenPos = transform.position;
        float freezeElapsed = 0f;

        while (freezeElapsed < freezeDuration)
        {
            freezeElapsed += Time.unscaledDeltaTime;

            // Sve intenzivnije drhtanje kako se bliži pucanju
            float intensity = (freezeElapsed / freezeDuration) * 0.08f;

            transform.position = frozenPos + new Vector3(
                Random.Range(-intensity, intensity),
                Random.Range(-intensity, intensity),
                0f);

            yield return null;
        }

        transform.position = frozenPos;

        // Faza 2: SHATTER!
        TriggerScreenShake(shakeIntensity * 2.5f, shakeDuration * 2f);
        PlayHitSound(true);

        // Spawn "ledene" krhotine
        for (int i = 0; i < shatterPieceCount; i++)
        {
            SpawnIceShard();
        }

        Destroy(gameObject);
    }


    private void SpawnIceShard()
    {
        GameObject shard = new GameObject("IceShard");
        shard.transform.position = transform.position;
        shard.transform.localScale = Vector3.one
            * Random.Range(debrisSize * 0.8f, debrisSize * 2f);

        SpriteRenderer sr = shard.AddComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteRenderer.sprite != null)
            sr.sprite = spriteRenderer.sprite;

        // Ledena boja sa varijacijom
        sr.color = new Color(
            Random.Range(0.6f, 0.9f),
            Random.Range(0.85f, 1f),
            1f,
            Random.Range(0.7f, 1f));

        sr.sortingOrder = spriteRenderer != null
            ? spriteRenderer.sortingOrder + 1 : 1;

        Rigidbody2D shardRB = shard.AddComponent<Rigidbody2D>();
        shardRB.gravityScale = Random.Range(2f, 4f);

        Vector2 dir = Random.insideUnitCircle.normalized;
        shardRB.linearVelocity = dir * Random.Range(debrisSpeed * 0.6f, debrisSpeed * 1.2f);
        shardRB.angularVelocity = Random.Range(-600f, 600f);

        DebrisFader fader = shard.AddComponent<DebrisFader>();
        fader.Init(debrisLifetime, sr);

        Destroy(shard, debrisLifetime + 0.1f);
    }


    // ─── 18. REWIND ─────────────────────────────────────────────────

    private void RewindEffect()
    {
        if (rewindPositions.Count < 10 || rewindActive) return;

        StartCoroutine(RewindRoutine());

        Debug.Log("[CLICK] ★ LEGENDARY ★ -> REWIND");
    }


    private IEnumerator RewindRoutine()
    {
        rewindActive = true;
        rb.simulated = false;

        if (trail != null) trail.enabled = false;

        // Rewind tint
        if (spriteRenderer != null)
            spriteRenderer.color = rewindTint;

        // Pusti pozicije unazad
        for (int i = rewindPositions.Count - 1; i >= 0; i--)
        {
            transform.position = rewindPositions[i];
            transform.eulerAngles = new Vector3(0f, 0f, rewindRotations[i]);

            // Spawn ghost na svakih nekoliko frejmova
            if (i % 4 == 0)
                SpawnGhostSprite();

            yield return new WaitForSecondsRealtime(rewindPlaybackSpeed);
        }

        // Vrati normalno
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        if (trail != null) trail.enabled = true;

        rb.simulated = true;
        rewindActive = false;

        // Očisti buffer i primeni silu
        rewindPositions.Clear();
        rewindRotations.Clear();

        rb.AddForce(Vector2.down * 8f, ForceMode2D.Impulse);
        AddRandomSpin();

        TriggerScreenShake(shakeIntensity, shakeDuration);
    }


    // ─── 19. MAGNET PULL ────────────────────────────────────────────

    private void MagnetPull(PointerEventData eventData)
    {
        Vector3 clickWorld = GetClickWorldPos(eventData);

        StartCoroutine(MagnetPullRoutine(clickWorld));

        Debug.Log("[CLICK] ★ LEGENDARY ★ -> MAGNET PULL");
    }


    private IEnumerator MagnetPullRoutine(Vector3 target)
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        float elapsed = 0f;
        Vector3 startPos = transform.position;

        // Privlačenje ka kursoru
        while (elapsed < magnetPullDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / magnetPullDuration);

            // Ease-in kretanje
            float eased = t * t;
            transform.position = Vector3.Lerp(startPos, target, eased);

            // Spin se ubrzava
            rb.angularVelocity = Mathf.Lerp(0f, 800f, t) * RandomSign();

            yield return null;
        }

        // LANSIRANJE u suprotnom smeru!
        TriggerScreenShake(shakeIntensity * 2f, shakeDuration * 1.5f);

        rb.gravityScale = Random.Range(minFallGravity, maxFallGravity);

        Vector2 launchDir = (transform.position - target).normalized;
        if (launchDir.sqrMagnitude < 0.01f)
            launchDir = Random.insideUnitCircle.normalized;

        // Dodaj upward bias
        launchDir.y = Mathf.Abs(launchDir.y) + 0.5f;
        launchDir.Normalize();

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(launchDir * magnetLaunchForce, ForceMode2D.Impulse);
        AddRandomSpin();
    }


    // ─── 20. CHAIN REACTION ─────────────────────────────────────────

    private void ChainReaction()
    {
        TriggerScreenShake(shakeIntensity * 2f, shakeDuration * 1.5f);
        PlayHitSound(true);

        // Jak self-kick
        Vector2 selfKick = new Vector2(
            Random.Range(-3f, 3f),
            Random.Range(4f, 8f));

        rb.AddForce(selfKick * chainSelfForce / selfKick.magnitude,
            ForceMode2D.Impulse);
        AddRandomSpin();

        // Pronađi susedne objekte
        Collider2D[] nearby = Physics2D.OverlapCircleAll(
            transform.position, chainRadius);

        int affected = 0;

        foreach (Collider2D other in nearby)
        {
            if (other.gameObject == gameObject) continue;

            Rigidbody2D otherRB = other.GetComponent<Rigidbody2D>();

            if (otherRB != null)
            {
                Vector2 dir = ((Vector2)(other.transform.position
                    - transform.position)).normalized;

                // Sila opada sa distancom
                float dist = Vector2.Distance(
                    transform.position, other.transform.position);

                float falloff = 1f - Mathf.Clamp01(dist / chainRadius);
                float force = chainForce * falloff;

                otherRB.AddForce(dir * force, ForceMode2D.Impulse);
                otherRB.angularVelocity += Random.Range(-300f, 300f);
            }

            // Aktiviraj pad na susednim ClickableObstacle-ima
            ClickableObstacle otherObstacle =
                other.GetComponent<ClickableObstacle>();

            if (otherObstacle != null && !otherObstacle.falling)
            {
                otherObstacle.StartFalling();
                affected++;
            }
        }

        // Impact burst na svakom pogođenom
        SpawnImpactBurst(transform.position);

        Debug.Log("[CLICK] ★ LEGENDARY ★ -> CHAIN REACTION (hit "
            + affected + " obstacles)");
    }


    // ─── 21. SHRINK SPIRAL ──────────────────────────────────────────

    private void ShrinkSpiral()
    {
        if (interactionLocked) return;
        interactionLocked = true;

        col.enabled = false;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        StartCoroutine(ShrinkSpiralRoutine());

        Debug.Log("[CLICK] ★ LEGENDARY ★ -> SHRINK SPIRAL");
    }


    private IEnumerator ShrinkSpiralRoutine()
    {
        float elapsed = 0f;
        Vector3 center = transform.position;
        Vector3 startScale = transform.localScale;

        while (elapsed < spiralDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / spiralDuration);

            // Spiralna putanja
            float angle = elapsed * spiralSpeed * Mathf.Deg2Rad;
            float currentRadius = spiralRadius * (1f - t);

            transform.position = center + new Vector3(
                Mathf.Cos(angle) * currentRadius,
                Mathf.Sin(angle) * currentRadius,
                0f);

            // Smanjivanje
            transform.localScale = Vector3.Lerp(
                startScale, Vector3.zero, t * t);

            // Rotacija
            transform.Rotate(0f, 0f, spiralSpeed * Time.unscaledDeltaTime);

            // Fade
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = c;
            }

            // Ghost svakih par frejmova
            if (Time.frameCount % 3 == 0)
                SpawnGhostSprite();

            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion


    // =================================================================
    //  VIZUELNI JUICE – svaki klik
    // =================================================================

    #region Visual Juice

    private void DoScalePunch()
    {
        if (scalePunchCoroutine != null)
            StopCoroutine(scalePunchCoroutine);

        scalePunchCoroutine = StartCoroutine(ScalePunchRoutine());
    }


    private IEnumerator ScalePunchRoutine()
    {
        Vector3 current = transform.localScale;
        Vector3 punched = current * scalePunchAmount;

        float half = scalePunchDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(current, punched, elapsed / half);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(punched, current, elapsed / half);
            yield return null;
        }

        transform.localScale = current;
        scalePunchCoroutine = null;
    }


    private void DoColorFlash()
    {
        if (spriteRenderer == null || flashPalette.Length == 0) return;

        if (colorFlashCoroutine != null)
            StopCoroutine(colorFlashCoroutine);

        Color flashColor = flashPalette[Random.Range(0, flashPalette.Length)];
        colorFlashCoroutine = StartCoroutine(ColorFlashRoutine(flashColor));
    }


    private IEnumerator ColorFlashRoutine(Color flashColor)
    {
        Color before = spriteRenderer.color;
        spriteRenderer.color = flashColor;

        yield return new WaitForSecondsRealtime(hitFlashDuration);

        float elapsed = 0f;
        float returnTime = hitFlashDuration * 2f;

        while (elapsed < returnTime)
        {
            elapsed += Time.unscaledDeltaTime;
            spriteRenderer.color = Color.Lerp(flashColor, before, elapsed / returnTime);
            yield return null;
        }

        spriteRenderer.color = before;
        colorFlashCoroutine = null;
    }


    /// <summary>
    /// Zlatni flash + scale punch kada se desi Legendary efekat.
    /// </summary>
    private void DoLegendaryFlash()
    {
        if (spriteRenderer == null) return;

        StartCoroutine(LegendaryFlashRoutine());
    }


    private IEnumerator LegendaryFlashRoutine()
    {
        Color gold = new Color(1f, 0.84f, 0f, 1f);
        Color before = spriteRenderer.color;

        // Brzi zlatni blic — 2 pulsa
        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.color = gold;
            yield return new WaitForSecondsRealtime(0.04f);
            spriteRenderer.color = before;
            yield return new WaitForSecondsRealtime(0.03f);
        }
    }


    /// <summary>
    /// Krug koji se pojavljuje na mestu klika i brzo nestaje.
    /// </summary>
    private void SpawnImpactBurst(Vector3 position)
    {
        if (cachedCircleSprite == null) return;

        GameObject burst = new GameObject("ImpactBurst");
        burst.transform.position = position;
        burst.transform.localScale = Vector3.zero;

        SpriteRenderer sr = burst.AddComponent<SpriteRenderer>();
        sr.sprite = cachedCircleSprite;
        sr.sortingOrder = 999;

        // Boja iz palete
        sr.color = flashPalette.Length > 0
            ? flashPalette[Random.Range(0, flashPalette.Length)]
            : Color.white;

        StartCoroutine(ImpactBurstRoutine(burst, sr));
    }


    private IEnumerator ImpactBurstRoutine(
        GameObject burst, SpriteRenderer sr)
    {
        float elapsed = 0f;
        Vector3 target = Vector3.one * impactBurstScale;
        Color startColor = sr.color;

        while (elapsed < impactBurstDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / impactBurstDuration);

            // Brzo raširi, postepeno nestani
            burst.transform.localScale = Vector3.Lerp(Vector3.zero, target, Mathf.Sqrt(t));

            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, t * t);
            sr.color = c;

            yield return null;
        }

        Destroy(burst);
    }

    #endregion


    // =================================================================
    //  SCREEN SHAKE (samo za specifične efekte)
    // =================================================================

    #region Screen Shake

    private void TriggerScreenShake(float intensity, float duration)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        ScreenShaker shaker = cam.GetComponent<ScreenShaker>();
        if (shaker == null)
            shaker = cam.gameObject.AddComponent<ScreenShaker>();

        shaker.Shake(intensity, duration);
    }

    #endregion


    // =================================================================
    //  GROUND COLLISION – squash-stretch + dust puff
    // =================================================================

    #region Ground Collision

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!falling) return;

        // Proveravamo da li je udarac odozgo (pod/tlo)
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                float impactForce = collision.relativeVelocity.magnitude;

                if (enableSquashStretch && impactForce > 2f)
                    DoSquashStretch(impactForce);

                if (enableDustPuff && impactForce > 3f)
                    SpawnDustPuff(contact.point);

                break;
            }
        }
    }


    private void DoSquashStretch(float impactForce)
    {
        if (squashCoroutine != null)
            StopCoroutine(squashCoroutine);

        squashCoroutine = StartCoroutine(SquashStretchRoutine(impactForce));
    }


    private IEnumerator SquashStretchRoutine(float impactForce)
    {
        Vector3 current = transform.localScale;

        // Intenzitet srazmerno sili udarca (capped)
        float intensity = Mathf.Clamp(impactForce / 15f, 0.1f, 0.3f);
        float squashY = 1f - intensity;
        float stretchX = 1f + intensity * 0.7f;

        Vector3 squashed = new Vector3(
            current.x * stretchX,
            current.y * squashY,
            current.z);

        float half = squashDuration * 0.5f;
        float elapsed = 0f;

        // Spljoštavanje
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(current, squashed, elapsed / half);
            yield return null;
        }

        // Vraćanje (sa malim overshoot za bounce feel)
        elapsed = 0f;
        Vector3 overshoot = new Vector3(
            current.x * (1f - intensity * 0.3f),
            current.y * (1f + intensity * 0.2f),
            current.z);

        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;

            // Overshoot pa nazad
            if (t < 0.5f)
                transform.localScale = Vector3.Lerp(squashed, overshoot, t * 2f);
            else
                transform.localScale = Vector3.Lerp(overshoot, current, (t - 0.5f) * 2f);

            yield return null;
        }

        transform.localScale = current;
        squashCoroutine = null;
    }


    private void SpawnDustPuff(Vector2 contactPoint)
    {
        for (int i = 0; i < dustParticleCount; i++)
        {
            GameObject dust = new GameObject("Dust");
            dust.transform.position = (Vector3)contactPoint;
            dust.transform.localScale = Vector3.one * Random.Range(0.06f, 0.14f);

            SpriteRenderer sr = dust.AddComponent<SpriteRenderer>();

            if (cachedCircleSprite != null)
                sr.sprite = cachedCircleSprite;

            sr.color = new Color(0.8f, 0.75f, 0.65f, 0.6f);
            sr.sortingOrder = spriteRenderer != null
                ? spriteRenderer.sortingOrder - 1 : 0;

            Rigidbody2D dustRB = dust.AddComponent<Rigidbody2D>();
            dustRB.gravityScale = -0.3f; // Lebdi nagore

            // Smer: uglavnom horizontalno od tačke udarca
            float angle = Random.Range(20f, 160f) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            dustRB.linearVelocity = dir * Random.Range(dustSpeed * 0.5f, dustSpeed);
            dustRB.angularVelocity = Random.Range(-100f, 100f);

            // Linear drag za usporavanje
            dustRB.linearDamping = 3f;

            DebrisFader fader = dust.AddComponent<DebrisFader>();
            fader.Init(dustLifetime, sr);

            Destroy(dust, dustLifetime + 0.1f);
        }
    }

    #endregion


    // =================================================================
    //  AUDIO
    // =================================================================

    #region Audio

    private void PlayHitSound(bool legendary)
    {
        if (audioSource == null) return;

        AudioClip[] clips = legendary && legendarySounds.Length > 0
            ? legendarySounds
            : hitSounds;

        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip, hitVolume);
    }

    #endregion


    // =================================================================
    //  POMOĆNE FUNKCIJE
    // =================================================================

    #region Helpers

    private float RandomSign()
    {
        return Random.value < 0.5f ? -1f : 1f;
    }


    private void AddRandomSpin()
    {
        rb.angularVelocity =
            Random.Range(minExtraSpin, maxExtraSpin) * RandomSign();
    }


    private void SpawnDebris(Vector3 origin)
    {
        GameObject debris = new GameObject("Debris");
        debris.transform.position = origin;
        debris.transform.localScale = Vector3.one * debrisSize;

        SpriteRenderer sr = debris.AddComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            sr.sprite = spriteRenderer.sprite;
            sr.color = originalColor;
        }
        else
        {
            sr.color = Color.white;
        }

        sr.sortingOrder = spriteRenderer != null
            ? spriteRenderer.sortingOrder + 1 : 1;

        Rigidbody2D debrisRB = debris.AddComponent<Rigidbody2D>();
        debrisRB.gravityScale = Random.Range(1.5f, 3f);

        Vector2 dir = Random.insideUnitCircle.normalized;
        debrisRB.linearVelocity = dir * Random.Range(debrisSpeed * 0.5f, debrisSpeed);
        debrisRB.angularVelocity = Random.Range(-500f, 500f);

        DebrisFader fader = debris.AddComponent<DebrisFader>();
        fader.Init(debrisLifetime, sr);

        Destroy(debris, debrisLifetime + 0.1f);
    }


    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Destroy(gameObject);
    }


    private IEnumerator QuickFadeDestroy(float duration)
    {
        if (spriteRenderer == null)
        {
            Destroy(gameObject);
            yield break;
        }

        col.enabled = false;

        float elapsed = 0f;
        Color startColor = spriteRenderer.color;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, t);
            spriteRenderer.color = c;

            transform.localScale = Vector3.Lerp(
                startScale, startScale * 0.3f, t);

            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion
}


// =====================================================================
//  DEBRIS FADER – fade-out + smanjivanje za krhotine
// =====================================================================

public class DebrisFader : MonoBehaviour
{
    private float lifetime;
    private SpriteRenderer sr;
    private float elapsed;
    private Color startColor;
    private Vector3 startScale;


    public void Init(float lifetime, SpriteRenderer renderer)
    {
        this.lifetime = lifetime;
        sr = renderer;

        if (sr != null) startColor = sr.color;
        startScale = transform.localScale;
    }


    private void Update()
    {
        if (sr == null) return;

        elapsed += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(elapsed / lifetime);

        Color c = startColor;
        c.a = Mathf.Lerp(startColor.a, 0f, t * t);
        sr.color = c;

        transform.localScale = Vector3.Lerp(
            startScale, startScale * 0.2f, t);
    }
}


// =====================================================================
//  GHOST FADER – fade-out za ghost sprite kopije
// =====================================================================

public class GhostFader : MonoBehaviour
{
    private SpriteRenderer sr;
    private float fadeTime;
    private float elapsed;
    private Color startColor;


    public void Init(float fadeTime)
    {
        this.fadeTime = fadeTime;
        sr = GetComponent<SpriteRenderer>();

        if (sr != null) startColor = sr.color;
    }


    private void Update()
    {
        if (sr == null) return;

        elapsed += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(elapsed / fadeTime);

        Color c = startColor;
        c.a = Mathf.Lerp(startColor.a, 0f, t);
        sr.color = c;

        // Lagano smanjivanje
        transform.localScale *= 1f - Time.unscaledDeltaTime * 0.5f;
    }
}


// =====================================================================
//  SCREEN SHAKER – camera shake (dodaje se samo kada treba)
// =====================================================================

public class ScreenShaker : MonoBehaviour
{
    private Vector3 originalPosition;
    private float currentIntensity;
    private float currentDuration;
    private float shakeElapsed;
    private bool isShaking;


    public void Shake(float intensity, float duration)
    {
        if (intensity > currentIntensity || !isShaking)
        {
            if (!isShaking)
                originalPosition = transform.localPosition;

            currentIntensity = intensity;
            currentDuration = duration;
            shakeElapsed = 0f;
            isShaking = true;
        }
    }


    private void Update()
    {
        if (!isShaking) return;

        shakeElapsed += Time.unscaledDeltaTime;

        if (shakeElapsed >= currentDuration)
        {
            transform.localPosition = originalPosition;
            isShaking = false;
            currentIntensity = 0f;
            return;
        }

        float remaining = 1f - (shakeElapsed / currentDuration);

        float x = Random.Range(-1f, 1f) * currentIntensity * remaining;
        float y = Random.Range(-1f, 1f) * currentIntensity * remaining;

        transform.localPosition = originalPosition + new Vector3(x, y, 0f);
    }


    private void OnDisable()
    {
        if (isShaking)
        {
            transform.localPosition = originalPosition;
            isShaking = false;
        }
    }
}