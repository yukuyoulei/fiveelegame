using UnityEngine;
using System.Collections;

namespace FiveElements.Unity
{
    public class MonsterObject : MonoBehaviour
    {
        [Header("Monster Stats")]
        public int MaxHealth = 50;
        public int Damage = 5;
        public float Speed = 0.5f;
        
        [Header("Visual")]
        public SpriteRenderer SpriteRenderer;
        public Slider HealthBar;
        
        private int _currentHealth;
        private bool _alive = true;
        private Vector2 _randomDirection;
        private float _directionChangeTime;
        
        private void Awake()
        {
            _currentHealth = MaxHealth;
            
            // 设置随机移动方向
            ChangeRandomDirection();
        }
        
        public void Initialize(int health, int damage, float speed)
        {
            MaxHealth = health;
            Damage = damage;
            Speed = speed;
            _currentHealth = health;
            
            // 根据世界距离调整属性
            if (OfflineGameManager.Instance != null)
            {
                int distance = OfflineGameManager.Instance.WorldDistance;
                MaxHealth = 50 + distance * 10;
                Damage = 5 + distance * 2;
                Speed = 0.5f + distance * 0.1f;
                _currentHealth = MaxHealth;
            }
            
            UpdateHealthBar();
        }
        
        public bool IsAlive() => _alive;
        
        public void TakeDamage(int damage)
        {
            if (!_alive) return;
            
            _currentHealth -= damage;
            UpdateHealthBar();
            
            // 显示伤害数字
            ShowDamageNumber(damage);
            
            // 播放受击动画
            PlayHitAnimation();
            
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            _alive = false;
            
            // 播放死亡动画
            PlayDeathAnimation();
            
            // 延迟销毁
            StartCoroutine(DestroyAfterDelay());
        }
        
        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
        
        private void Update()
        {
            if (!_alive) return;
            
            // 随机移动
            RandomMove();
            
            // 更新方向变化时间
            _directionChangeTime -= Time.deltaTime;
            if (_directionChangeTime <= 0)
            {
                ChangeRandomDirection();
            }
        }
        
        private void RandomMove()
        {
            // 简单的随机移动
            transform.Translate(_randomDirection * Speed * Time.deltaTime);
            
            // 限制在场景范围内
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -7f, 7f);
            pos.y = Mathf.Clamp(pos.y, -5f, 5f);
            transform.position = pos;
        }
        
        private void ChangeRandomDirection()
        {
            float angle = Random.Range(0f, 2f * Mathf.PI);
            _randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            _directionChangeTime = Random.Range(2f, 4f);
        }
        
        private void UpdateHealthBar()
        {
            if (HealthBar != null)
            {
                HealthBar.value = (float)_currentHealth / MaxHealth;
            }
        }
        
        private void ShowDamageNumber(int damage)
        {
            // 这里可以实例化伤害数字预制体
            Debug.Log($"Damage: -{damage}");
        }
        
        private void PlayHitAnimation()
        {
            if (SpriteRenderer != null)
            {
                // 闪烁效果
                StartCoroutine(FlashEffect());
            }
        }
        
        private void PlayDeathAnimation()
        {
            if (SpriteRenderer != null)
            {
                // 淡出效果
                StartCoroutine(FadeOutEffect());
            }
        }
        
        private IEnumerator FlashEffect()
        {
            Color originalColor = SpriteRenderer.color;
            SpriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            SpriteRenderer.color = originalColor;
        }
        
        private IEnumerator FadeOutEffect()
        {
            float fadeTime = 1f;
            float elapsed = 0f;
            
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = 1f - (elapsed / fadeTime);
                Color color = SpriteRenderer.color;
                color.a = alpha;
                SpriteRenderer.color = color;
                yield return null;
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 碰撞到边界时改变方向
            if (collision.gameObject.CompareTag("Boundary"))
            {
                ChangeRandomDirection();
            }
        }
    }
}