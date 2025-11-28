using UnityEngine;
using FiveElements.Unity.Managers;

namespace FiveElements.Unity
{
    public class PlayerController : MonoBehaviour
    {
        private OfflineGameManager _gameManager;
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        
        [Header("Movement")]
        public float MoveSpeed = 2f;
        
        [Header("Combat")]
        public float AttackRange = 2f;
        public float AttackCooldown = 0.5f;
        private float _lastAttackTime;
        
        [Header("Collection")]
        public float CollectRange = 1.5f;
        public float CollectTime = 2f;
        private bool _isCollecting = false;
        private ResourceObject _targetResource;
        
        private Vector2 _moveInput;
        private bool _isMoving = false;
        private bool _isAttacking = false;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public void Initialize(OfflineGameManager gameManager)
        {
            _gameManager = gameManager;
        }
        
        private void Update()
        {
            HandleInput();
            CheckInteractions();
            UpdateAnimations();
        }
        
        private void FixedUpdate()
        {
            MovePlayer();
        }
        
        private void HandleInput()
        {
            // 键盘输入
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            _moveInput = new Vector2(horizontal, vertical).normalized;
            
            // 攻击输入
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryAttack();
            }
            
            // 采集输入
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryCollect();
            }
            
            // 对话输入
            if (Input.GetKeyDown(KeyCode.F))
            {
                TryTalk();
            }
            
            // 场景切换输入
            if (Input.GetKeyDown(KeyCode.W) && transform.position.y >= 4f)
            {
                TryChangeScene(0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) && transform.position.y <= -4f)
            {
                TryChangeScene(0, -1);
            }
            else if (Input.GetKeyDown(KeyCode.A) && transform.position.x <= -6f)
            {
                TryChangeScene(-1, 0);
            }
            else if (Input.GetKeyDown(KeyCode.D) && transform.position.x >= 6f)
            {
                TryChangeScene(1, 0);
            }
        }
        
        private void MovePlayer()
        {
            if (_isCollecting) return;
            
            Vector2 movement = _moveInput * MoveSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(_rigidbody.position + movement);
            
            _isMoving = movement.magnitude > 0.01f;
            
            // 限制移动范围
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -7f, 7f);
            pos.y = Mathf.Clamp(pos.y, -5f, 5f);
            transform.position = pos;
        }
        
        private void CheckInteractions()
        {
            // 检查附近的资源
            ResourceObject nearestResource = FindNearest<ResourceObject>(CollectRange);
            if (nearestResource != null && nearestResource != _targetResource)
            {
                _targetResource = nearestResource;
            }
            
            // 检查附近的怪物
            MonsterObject nearestMonster = FindNearest<MonsterObject>(AttackRange);
            if (nearestMonster != null && !_isAttacking)
            {
                AutoAttackMonster(nearestMonster);
            }
        }
        
        private T FindNearest<T>(float range) where T : MonoBehaviour
        {
            T[] objects = FindObjectsOfType<T>();
            T nearest = null;
            float minDistance = range;
            
            foreach (T obj in objects)
            {
                float distance = Vector2.Distance(transform.position, obj.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = obj;
                }
            }
            
            return nearest;
        }
        
        private void TryAttack()
        {
            if (Time.time < _lastAttackTime + AttackCooldown) return;
            
            MonsterObject nearestMonster = FindNearest<MonsterObject>(AttackRange);
            if (nearestMonster != null)
            {
                AttackMonster(nearestMonster);
            }
        }
        
        private void AutoAttackMonster(MonsterObject monster)
        {
            if (Time.time >= _lastAttackTime + AttackCooldown)
            {
                AttackMonster(monster);
            }
        }
        
        private void AttackMonster(MonsterObject monster)
        {
            _isAttacking = true;
            _lastAttackTime = Time.time;
            
            // 计算伤害
            int damage = 10 + (_gameManager.GetComponent<SkillManager>().GetBodyLevel() * 2);
            
            // 造成伤害
            monster.TakeDamage(damage);
            
            // 播放攻击动画
            if (_animator != null)
            {
                _animator.SetTrigger("Attack");
            }
            
            // 创建攻击效果
            CreateAttackEffect(monster.transform.position);
            
            // 检查怪物是否死亡
            if (!monster.IsAlive())
            {
                _gameManager.KillMonster(monster);
            }
            
            StartCoroutine(ResetAttackState());
        }
        
        private System.Collections.IEnumerator ResetAttackState()
        {
            yield return new WaitForSeconds(AttackCooldown);
            _isAttacking = false;
        }
        
        private void TryCollect()
        {
            if (_isCollecting) return;
            
            if (_targetResource != null && !_targetResource.IsCollected())
            {
                StartCoroutine(CollectResource(_targetResource));
            }
        }
        
        private System.Collections.IEnumerator CollectResource(ResourceObject resource)
        {
            _isCollecting = true;
            
            float collectTime = resource.CollectTime;
            float elapsed = 0f;
            
            while (elapsed < collectTime && Vector2.Distance(transform.position, resource.transform.position) <= CollectRange)
            {
                elapsed += Time.deltaTime;
                resource.UpdateProgress(elapsed / collectTime);
                yield return null;
            }
            
            if (elapsed >= collectTime)
            {
                _gameManager.CollectResource(resource);
            }
            else
            {
                resource.ResetProgress();
            }
            
            _isCollecting = false;
            _targetResource = null;
        }
        
        private void TryTalk()
        {
            NPCObject nearestNPC = FindNearest<NPCObject>(1.5f);
            if (nearestNPC != null && !nearestNPC.Talked)
            {
                _gameManager.TalkToNPC(nearestNPC);
            }
        }
        
        private void TryChangeScene(int directionX, int directionY)
        {
            if (_gameManager.CanChangeScene())
            {
                _gameManager.ChangeScene(directionX, directionY);
            }
            else
            {
                // 显示提示信息
                Debug.Log("需要完成当前任务才能离开场景");
            }
        }
        
        private void UpdateAnimations()
        {
            if (_animator != null)
            {
                _animator.SetBool("IsMoving", _isMoving);
                _animator.SetBool("IsAttacking", _isAttacking);
            }
            
            // 翻转精灵
            if (_moveInput.x > 0)
            {
                if (_spriteRenderer != null)
                    _spriteRenderer.flipX = false;
            }
            else if (_moveInput.x < 0)
            {
                if (_spriteRenderer != null)
                    _spriteRenderer.flipX = true;
            }
        }
        
        private void CreateAttackEffect(Vector3 position)
        {
            // 这里可以创建攻击特效
            Debug.Log($"Attack effect at {position}");
        }
        
        private void OnDrawGizmosSelected()
        {
            // 绘制攻击范围
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
            
            // 绘制采集范围
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, CollectRange);
        }
    }
}