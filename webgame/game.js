class FiveElementsGame {
    constructor() {
        this.canvas = document.getElementById('gameCanvas');
        this.ctx = this.canvas.getContext('2d');
        this.canvas.width = 480;
        this.canvas.height = 640;
        
        // 游戏状态
        this.layers = {
            background: { x: 0, y: 0, section: 'middle' },
            middle: { x: 0, y: 0, section: 'middle' },
            foreground: { x: 0, y: 0, section: 'middle' }
        };
        
        this.player = {
            x: 240,
            y: 320,
            width: 32,
            height: 32,
            speed: 2,
            health: 100,
            maxHealth: 100,
            elements: {
                metal: 10,
                wood: 10,
                water: 10,
                fire: 10,
                earth: 10
            },
            skills: {
                mind: { level: 0, direction: null },
                body: { level: 0, direction: null }
            },
            isMoving: false,
            isAttacking: false,
            isCollecting: false,
            facing: 'right'
        };
        
        this.world = {
            originX: 0,
            originY: 0,
            distance: 0
        };
        
        this.currentTask = null;
        this.taskComplete = false;
        this.objects = [];
        this.npcs = [];
        this.monsters = [];
        this.resources = [];
        this.particles = [];
        this.floatingTexts = [];
        
        this.keys = {};
        this.transitioning = false;
        this.breakthroughInProgress = false;
        
        this.init();
    }
    
    init() {
        this.loadGameState();
        this.setupEventListeners();
        this.generateTask();
        this.gameLoop();
    }
    
    loadGameState() {
        const savedState = localStorage.getItem('fiveElementsGameState');
        if (savedState) {
            const state = JSON.parse(savedState);
            this.player = { ...this.player, ...state.player };
            this.world = { ...this.world, ...state.world };
            this.player.skills = state.player.skills || this.player.skills;
        }
    }
    
    saveGameState() {
        const state = {
            player: {
                x: this.player.x,
                y: this.player.y,
                health: this.player.health,
                elements: this.player.elements,
                skills: this.player.skills
            },
            world: this.world
        };
        localStorage.setItem('fiveElementsGameState', JSON.stringify(state));
    }
    
    setupEventListeners() {
        window.addEventListener('keydown', (e) => {
            this.keys[e.key] = true;
            e.preventDefault();
        });
        
        window.addEventListener('keyup', (e) => {
            this.keys[e.key] = false;
            e.preventDefault();
        });
    }
    
    generateTask() {
        const taskTypes = ['collect', 'kill', 'talk'];
        const type = taskTypes[Math.floor(Math.random() * taskTypes.length)];
        
        switch(type) {
            case 'collect':
                this.currentTask = {
                    type: 'collect',
                    description: '采集一次资源',
                    completed: false
                };
                this.spawnResource();
                break;
            case 'kill':
                this.currentTask = {
                    type: 'kill',
                    description: '击杀一个怪物',
                    completed: false
                };
                this.spawnMonster();
                break;
            case 'talk':
                this.currentTask = {
                    type: 'talk',
                    description: '与NPC对话',
                    completed: false
                };
                this.spawnNPC();
                break;
        }
        
        this.updateTaskDisplay();
    }
    
    spawnResource() {
        const elements = ['metal', 'wood', 'water', 'fire', 'earth'];
        const element = elements[Math.floor(Math.random() * elements.length)];
        const amount = Math.floor(5 + this.world.distance * 2);
        const collectTime = 2000 + this.world.distance * 500;
        
        this.resources.push({
            x: Math.random() * (this.canvas.width - 40) + 20,
            y: Math.random() * (this.canvas.height - 200) + 100,
            width: 40,
            height: 40,
            type: element,
            amount: amount,
            collectTime: collectTime,
            collected: false,
            collecting: false,
            progress: 0
        });
    }
    
    spawnMonster() {
        const health = 50 + this.world.distance * 10;
        const damage = 5 + this.world.distance * 2;
        const speed = 0.5 + this.world.distance * 0.1;
        
        this.monsters.push({
            x: Math.random() * (this.canvas.width - 40) + 20,
            y: Math.random() * (this.canvas.height - 200) + 100,
            width: 40,
            height: 40,
            health: health,
            maxHealth: health,
            damage: damage,
            speed: speed,
            direction: Math.random() * Math.PI * 2,
            alive: true
        });
    }
    
    spawnNPC() {
        this.npcs.push({
            x: Math.random() * (this.canvas.width - 40) + 20,
            y: Math.random() * (this.canvas.height - 200) + 100,
            width: 40,
            height: 40,
            talked: false,
            message: '你好，勇敢的冒险者！'
        });
    }
    
    update() {
        if (this.transitioning) return;
        
        this.handleInput();
        this.updateMonsters();
        this.updateParticles();
        this.updateFloatingTexts();
        this.checkCollisions();
        this.updateDistance();
        this.saveGameState();
    }
    
    handleInput() {
        let dx = 0;
        let dy = 0;
        
        if (this.keys['ArrowLeft'] || this.keys['a']) {
            dx = -this.player.speed;
            this.player.facing = 'left';
        }
        if (this.keys['ArrowRight'] || this.keys['d']) {
            dx = this.player.speed;
            this.player.facing = 'right';
        }
        if (this.keys['ArrowUp'] || this.keys['w']) {
            dy = -this.player.speed;
        }
        if (this.keys['ArrowDown'] || this.keys['s']) {
            dy = this.player.speed;
        }
        
        // 检查是否可以切换场景
        if (this.canChangeScene()) {
            // 左移到边界
            if (this.player.x <= 50 && dx < 0) {
                this.changeScene('left');
                return;
            }
            // 右移到边界
            if (this.player.x >= this.canvas.width - 50 && dx > 0) {
                this.changeScene('right');
                return;
            }
            // 上移到边界
            if (this.player.y <= 50 && dy < 0) {
                this.changeScene('up');
                return;
            }
            // 下移到边界
            if (this.player.y >= this.canvas.height - 50 && dy > 0) {
                this.changeScene('down');
                return;
            }
        }
        
        // 移动玩家
        this.player.x = Math.max(20, Math.min(this.canvas.width - 20, this.player.x + dx));
        this.player.y = Math.max(20, Math.min(this.canvas.height - 20, this.player.y + dy));
        
        this.player.isMoving = dx !== 0 || dy !== 0;
    }
    
    canChangeScene() {
        return this.currentTask && this.currentTask.completed;
    }
    
    changeScene(direction) {
        if (this.transitioning) return;
        
        this.transitioning = true;
        
        // 更新世界坐标
        switch(direction) {
            case 'left':
                this.world.originX -= 1;
                break;
            case 'right':
                this.world.originX += 1;
                break;
            case 'up':
                this.world.originY -= 1;
                break;
            case 'down':
                this.world.originY += 1;
                break;
        }
        
        // 平滑过渡动画
        this.animateTransition(direction, () => {
            this.transitioning = false;
            this.player.x = this.canvas.width / 2;
            this.player.y = this.canvas.height / 2;
            
            // 清除当前场景对象
            this.objects = [];
            this.npcs = [];
            this.monsters = [];
            this.resources = [];
            
            // 生成新任务
            this.generateTask();
        });
    }
    
    animateTransition(direction, callback) {
        const duration = 500;
        const startTime = Date.now();
        
        const animate = () => {
            const elapsed = Date.now() - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            // 绘制过渡效果
            this.drawTransition(direction, progress);
            
            if (progress < 1) {
                requestAnimationFrame(animate);
            } else {
                callback();
            }
        };
        
        animate();
    }
    
    drawTransition(direction, progress) {
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.3)';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // 绘制移动效果
        const offset = progress * 100;
        this.ctx.save();
        
        switch(direction) {
            case 'left':
                this.ctx.translate(offset, 0);
                break;
            case 'right':
                this.ctx.translate(-offset, 0);
                break;
            case 'up':
                this.ctx.translate(0, offset);
                break;
            case 'down':
                this.ctx.translate(0, -offset);
                break;
        }
        
        this.ctx.restore();
    }
    
    updateMonsters() {
        this.monsters.forEach(monster => {
            if (!monster.alive) return;
            
            // 随机移动
            monster.direction += (Math.random() - 0.5) * 0.2;
            monster.x += Math.cos(monster.direction) * monster.speed;
            monster.y += Math.sin(monster.direction) * monster.speed;
            
            // 边界检测
            if (monster.x <= 20 || monster.x >= this.canvas.width - 20) {
                monster.direction = Math.PI - monster.direction;
            }
            if (monster.y <= 20 || monster.y >= this.canvas.height - 20) {
                monster.direction = -monster.direction;
            }
            
            monster.x = Math.max(20, Math.min(this.canvas.width - 20, monster.x));
            monster.y = Math.max(20, Math.min(this.canvas.height - 20, monster.y));
        });
    }
    
    updateParticles() {
        this.particles = this.particles.filter(particle => {
            particle.x += particle.vx;
            particle.y += particle.vy;
            particle.life--;
            return particle.life > 0;
        });
    }
    
    updateFloatingTexts() {
        this.floatingTexts = this.floatingTexts.filter(text => {
            text.y -= 1;
            text.life--;
            return text.life > 0;
        });
    }
    
    checkCollisions() {
        // 检查资源采集
        this.resources.forEach(resource => {
            if (resource.collected) return;
            
            const dist = Math.hypot(this.player.x - resource.x, this.player.y - resource.y);
            if (dist < 40) {
                if (!resource.collecting) {
                    resource.collecting = true;
                    this.startCollecting(resource);
                }
            } else {
                resource.collecting = false;
                resource.progress = 0;
            }
        });
        
        // 检查怪物攻击
        this.monsters.forEach(monster => {
            if (!monster.alive) return;
            
            const dist = Math.hypot(this.player.x - monster.x, this.player.y - monster.y);
            if (dist < 50) {
                if (!this.player.isAttacking) {
                    this.attackMonster(monster);
                }
            }
        });
        
        // 检查NPC对话
        this.npcs.forEach(npc => {
            if (npc.talked) return;
            
            const dist = Math.hypot(this.player.x - npc.x, this.player.y - npc.y);
            if (dist < 40) {
                if (this.keys[' ']) {
                    this.talkToNPC(npc);
                }
            }
        });
    }
    
    startCollecting(resource) {
        const collectInterval = setInterval(() => {
            if (!resource.collecting || resource.collected) {
                clearInterval(collectInterval);
                return;
            }
            
            resource.progress += 16; // 约60fps
            const progress = resource.progress / resource.collectTime;
            
            // 更新资源条
            document.getElementById('resourceFill').style.width = `${progress * 100}%`;
            
            if (resource.progress >= resource.collectTime) {
                this.collectResource(resource);
                clearInterval(collectInterval);
            }
        }, 16);
    }
    
    collectResource(resource) {
        resource.collected = true;
        
        // 增加元素
        const elementName = this.getElementName(resource.type);
        this.player.elements[resource.type] += resource.amount;
        
        // 完成任务
        if (this.currentTask && this.currentTask.type === 'collect') {
            this.currentTask.completed = true;
            this.showFloatingText(`任务完成！+${resource.amount} ${elementName}`, resource.x, resource.y);
        } else {
            this.showFloatingText(`+${resource.amount} ${elementName}`, resource.x, resource.y);
        }
        
        this.updateInventory();
        this.updateTaskDisplay();
        
        // 创建粒子效果
        this.createParticles(resource.x, resource.y, this.getElementColor(resource.type));
    }
    
    attackMonster(monster) {
        this.player.isAttacking = true;
        
        const attackInterval = setInterval(() => {
            if (!this.player.isAttacking || !monster.alive) {
                clearInterval(attackInterval);
                this.player.isAttacking = false;
                return;
            }
            
            const dist = Math.hypot(this.player.x - monster.x, this.player.y - monster.y);
            if (dist < 50) {
                const damage = 10 + (this.player.skills.body.level * 2);
                monster.health -= damage;
                
                // 创建攻击效果
                this.createParticles(monster.x, monster.y, '#ff6b6b');
                this.showFloatingText(`-${damage}`, monster.x, monster.y - 20);
                
                if (monster.health <= 0) {
                    monster.alive = false;
                    this.killMonster(monster);
                    clearInterval(attackInterval);
                    this.player.isAttacking = false;
                }
            }
        }, 500 / (1 + this.player.skills.body.level * 0.1)); // 外功提升攻击速度
    }
    
    killMonster(monster) {
        // 随机获得元素
        const elements = ['metal', 'wood', 'water', 'fire', 'earth'];
        const element = elements[Math.floor(Math.random() * elements.length)];
        const amount = Math.floor(3 + this.world.distance * 1.5 + this.player.skills.mind.level * 0.5);
        
        this.player.elements[element] += amount;
        
        // 完成任务
        if (this.currentTask && this.currentTask.type === 'kill') {
            this.currentTask.completed = true;
            this.showFloatingText(`任务完成！+${amount} ${this.getElementName(element)}`, monster.x, monster.y);
        } else {
            this.showFloatingText(`+${amount} ${this.getElementName(element)}`, monster.x, monster.y);
        }
        
        this.updateInventory();
        this.updateTaskDisplay();
        
        // 创建死亡效果
        this.createParticles(monster.x, monster.y, '#ff0000');
    }
    
    talkToNPC(npc) {
        if (npc.talked) return;
        
        npc.talked = true;
        
        // 随机获得元素
        const elements = ['metal', 'wood', 'water', 'fire', 'earth'];
        const element = elements[Math.floor(Math.random() * elements.length)];
        const amount = Math.floor(5 + this.world.distance + this.player.skills.mind.level);
        
        this.player.elements[element] += amount;
        
        // 完成任务
        if (this.currentTask && this.currentTask.type === 'talk') {
            this.currentTask.completed = true;
            this.showFloatingText(`任务完成！+${amount} ${this.getElementName(element)}`, npc.x, npc.y);
        } else {
            this.showFloatingText(`+${amount} ${this.getElementName(element)}`, npc.x, npc.y);
        }
        
        this.updateInventory();
        this.updateTaskDisplay();
        
        // 显示对话
        this.showDialog(npc.message);
    }
    
    createParticles(x, y, color) {
        for (let i = 0; i < 10; i++) {
            this.particles.push({
                x: x,
                y: y,
                vx: (Math.random() - 0.5) * 4,
                vy: (Math.random() - 0.5) * 4,
                color: color,
                life: 30
            });
        }
    }
    
    showFloatingText(text, x, y) {
        this.floatingTexts.push({
            text: text,
            x: x,
            y: y,
            life: 60
        });
    }
    
    showDialog(message) {
        const dialogBox = document.getElementById('dialogBox');
        dialogBox.textContent = message;
        dialogBox.style.display = 'block';
        
        setTimeout(() => {
            dialogBox.style.display = 'none';
        }, 3000);
    }
    
    updateDistance() {
        this.world.distance = Math.abs(this.world.originX) + Math.abs(this.world.originY);
    }
    
    getElementName(element) {
        const names = {
            metal: '金',
            wood: '木',
            water: '水',
            fire: '火',
            earth: '土'
        };
        return names[element] || element;
    }
    
    getElementColor(element) {
        const colors = {
            metal: '#C0C0C0',
            wood: '#8B4513',
            water: '#4682B4',
            fire: '#FF4500',
            earth: '#8B7355'
        };
        return colors[element] || '#FFFFFF';
    }
    
    updateCoordinates() {
        document.getElementById('coordinates').textContent = `坐标: (${this.world.originX}, ${this.world.originY})`;
    }
    
    updateTaskDisplay() {
        const taskDisplay = document.getElementById('taskDisplay');
        if (this.currentTask) {
            const status = this.currentTask.completed ? '✓' : '○';
            taskDisplay.textContent = `${status} ${this.currentTask.description}`;
            
            if (!this.currentTask.completed) {
                taskDisplay.textContent += '\n完成场景任务后才能离开';
            }
        }
    }
    
    updateInventory() {
        const inventory = document.getElementById('inventory');
        let text = '背包: ';
        
        Object.entries(this.player.elements).forEach(([element, amount]) => {
            if (amount > 0) {
                text += `${this.getElementName(element)}:${amount} `;
            }
        });
        
        if (text === '背包: ') {
            text = '背包: 空';
        }
        
        inventory.textContent = text;
    }
    
    showSkillPanel() {
        const panel = document.getElementById('skillPanel');
        const info = document.getElementById('skillInfo');
        
        let html = '';
        html += `<p>心法等级: ${this.player.skills.mind.level}</p>`;
        html += `<p>外功等级: ${this.player.skills.body.level}</p>`;
        html += `<p>总元素: ${this.getTotalElements()}</p>`;
        
        info.innerHTML = html;
        panel.style.display = 'block';
    }
    
    closeSkillPanel() {
        document.getElementById('skillPanel').style.display = 'none';
    }
    
    getTotalElements() {
        return Object.values(this.player.elements).reduce((sum, amount) => sum + amount, 0);
    }
    
    upgradeSkill(type) {
        const skill = this.player.skills[type];
        
        if (!skill.direction) {
            // 选择方向
            skill.direction = type;
            this.showFloatingText(`选择了${type === 'mind' ? '心法' : '外功'}方向`, this.player.x, this.player.y - 30);
            return;
        }
        
        // 检查是否需要突破
        if (skill.level > 0 && skill.level % 5 === 0) {
            this.showBreakthroughPanel(type);
            return;
        }
        
        // 消耗元素升级
        const cost = (skill.level + 1) * 5;
        if (this.getTotalElements() >= cost) {
            this.deductElements(cost);
            skill.level++;
            this.showFloatingText(`${type === 'mind' ? '心法' : '外功'}升级到${skill.level}级`, this.player.x, this.player.y - 30);
            this.showSkillPanel();
        } else {
            this.showFloatingText('元素不足', this.player.x, this.player.y - 30);
        }
    }
    
    showBreakthroughPanel(type) {
        const panel = document.getElementById('breakthroughPanel');
        const skill = this.player.skills[type];
        
        document.getElementById('breakthroughLevel').textContent = skill.level;
        document.getElementById('breakthroughCost').textContent = skill.level * 3;
        document.getElementById('breakthroughChance').textContent = Math.min(50 + skill.level * 5, 90);
        
        panel.style.display = 'block';
        this.currentBreakthroughType = type;
    }
    
    closeBreakthroughPanel() {
        document.getElementById('breakthroughPanel').style.display = 'none';
    }
    
    startBreakthrough() {
        if (this.breakthroughInProgress) return;
        
        this.breakthroughInProgress = true;
        const skill = this.player.skills[this.currentBreakthroughType];
        const cost = skill.level * 3;
        
        const breakthroughInterval = setInterval(() => {
            if (!this.breakthroughInProgress) {
                clearInterval(breakthroughInterval);
                return;
            }
            
            if (this.getTotalElements() < cost) {
                this.breakthroughInProgress = false;
                this.showFloatingText('元素不足，突破失败', this.player.x, this.player.y - 30);
                clearInterval(breakthroughInterval);
                return;
            }
            
            this.deductElements(cost);
            
            // 随机突破
            const chance = Math.min(50 + skill.level * 5, 90);
            if (Math.random() * 100 < chance) {
                skill.level++;
                this.breakthroughInProgress = false;
                this.showFloatingText(`突破成功！${this.currentBreakthroughType === 'mind' ? '心法' : '外功'}提升到${skill.level}级`, this.player.x, this.player.y - 30);
                this.closeBreakthroughPanel();
                clearInterval(breakthroughInterval);
            } else {
                // 更新进度条
                const progress = Math.random() * 100;
                document.getElementById('breakthroughProgress').style.width = `${progress}%`;
            }
        }, 1000);
    }
    
    stopBreakthrough() {
        this.breakthroughInProgress = false;
        document.getElementById('breakthroughProgress').style.width = '0%';
    }
    
    deductElements(amount) {
        const total = this.getTotalElements();
        const ratio = amount / total;
        
        Object.keys(this.player.elements).forEach(element => {
            this.player.elements[element] = Math.max(0, Math.floor(this.player.elements[element] * (1 - ratio)));
        });
        
        this.updateInventory();
    }
    
    render() {
        // 清空画布
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        
        // 绘制背景层
        this.drawLayer('background', 0.3);
        
        // 绘制中景层
        this.drawLayer('middle', 0.6);
        
        // 绘制前景层
        this.drawLayer('foreground', 1.0);
        
        // 绘制游戏对象
        this.drawObjects();
        
        // 绘制玩家
        this.drawPlayer();
        
        // 绘制粒子效果
        this.drawParticles();
        
        // 绘制漂浮文字
        this.drawFloatingTexts();
        
        // 更新UI
        this.updateCoordinates();
    }
    
    drawLayer(layer, opacity) {
        const gradient = this.ctx.createLinearGradient(0, 0, 0, this.canvas.height);
        
        switch(layer) {
            case 'background':
                gradient.addColorStop(0, `rgba(135, 206, 235, ${opacity})`); // 天空蓝
                gradient.addColorStop(0.6, `rgba(255, 255, 200, ${opacity})`); // 浅黄
                gradient.addColorStop(1, `rgba(34, 139, 34, ${opacity})`); // 森林绿
                break;
            case 'middle':
                gradient.addColorStop(0, `rgba(255, 255, 200, ${opacity * 0.3})`);
                gradient.addColorStop(0.5, `rgba(34, 139, 34, ${opacity})`);
                gradient.addColorStop(1, `rgba(101, 67, 33, ${opacity})`); // 棕色地面
                break;
            case 'foreground':
                gradient.addColorStop(0, `rgba(34, 139, 34, ${opacity * 0.2})`);
                gradient.addColorStop(0.7, `rgba(101, 67, 33, ${opacity * 0.8})`);
                gradient.addColorStop(1, `rgba(139, 90, 43, ${opacity})`); // 深棕色
                break;
        }
        
        this.ctx.fillStyle = gradient;
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // 添加一些装饰性元素
        this.drawLayerDecorations(layer, opacity);
    }
    
    drawLayerDecorations(layer, opacity) {
        this.ctx.save();
        
        switch(layer) {
            case 'background':
                // 绘制远山
                this.ctx.fillStyle = `rgba(100, 100, 100, ${opacity * 0.3})`;
                for (let i = 0; i < 5; i++) {
                    const x = i * 120 + this.world.originX * 10;
                    const y = 100 + Math.sin(i) * 20;
                    this.drawMountain(x, y, 80, 60);
                }
                break;
                
            case 'middle':
                // 绘制树木
                this.ctx.fillStyle = `rgba(34, 139, 34, ${opacity})`;
                for (let i = 0; i < 8; i++) {
                    const x = i * 60 + this.world.originX * 20;
                    const y = 200 + Math.sin(i * 0.5) * 30;
                    this.drawTree(x, y);
                }
                break;
                
            case 'foreground':
                // 绘制草地纹理
                this.ctx.strokeStyle = `rgba(34, 139, 34, ${opacity * 0.5})`;
                this.ctx.lineWidth = 1;
                for (let i = 0; i < 20; i++) {
                    const x = Math.random() * this.canvas.width;
                    const y = 400 + Math.random() * 200;
                    this.drawGrass(x, y);
                }
                break;
        }
        
        this.ctx.restore();
    }
    
    drawMountain(x, y, width, height) {
        this.ctx.beginPath();
        this.ctx.moveTo(x, y + height);
        this.ctx.lineTo(x + width / 2, y);
        this.ctx.lineTo(x + width, y + height);
        this.ctx.closePath();
        this.ctx.fill();
    }
    
    drawTree(x, y) {
        // 树干
        this.ctx.fillStyle = '#8B4513';
        this.ctx.fillRect(x - 5, y, 10, 30);
        
        // 树叶
        this.ctx.fillStyle = '#228B22';
        this.ctx.beginPath();
        this.ctx.arc(x, y - 10, 20, 0, Math.PI * 2);
        this.ctx.fill();
    }
    
    drawGrass(x, y) {
        this.ctx.beginPath();
        this.ctx.moveTo(x, y);
        this.ctx.lineTo(x - 2, y - 8);
        this.ctx.moveTo(x, y);
        this.ctx.lineTo(x + 2, y - 8);
        this.ctx.stroke();
    }
    
    drawObjects() {
        // 绘制资源
        this.resources.forEach(resource => {
            if (resource.collected) return;
            
            this.ctx.fillStyle = this.getElementColor(resource.type);
            this.ctx.fillRect(resource.x - resource.width/2, resource.y - resource.height/2, resource.width, resource.height);
            
            // 绘制采集进度
            if (resource.collecting) {
                this.ctx.fillStyle = 'rgba(0, 0, 0, 0.5)';
                this.ctx.fillRect(resource.x - 20, resource.y - 30, 40, 5);
                this.ctx.fillStyle = '#4CAF50';
                this.ctx.fillRect(resource.x - 20, resource.y - 30, 40 * (resource.progress / resource.collectTime), 5);
            }
        });
        
        // 绘制怪物
        this.monsters.forEach(monster => {
            if (!monster.alive) return;
            
            this.ctx.fillStyle = '#FF6B6B';
            this.ctx.fillRect(monster.x - monster.width/2, monster.y - monster.height/2, monster.width, monster.height);
            
            // 绘制血条
            this.ctx.fillStyle = 'rgba(0, 0, 0, 0.5)';
            this.ctx.fillRect(monster.x - 20, monster.y - 30, 40, 5);
            this.ctx.fillStyle = '#FF0000';
            this.ctx.fillRect(monster.x - 20, monster.y - 30, 40 * (monster.health / monster.maxHealth), 5);
        });
        
        // 绘制NPC
        this.npcs.forEach(npc => {
            if (npc.talked) return;
            
            this.ctx.fillStyle = '#4169E1';
            this.ctx.fillRect(npc.x - npc.width/2, npc.y - npc.height/2, npc.width, npc.height);
            
            // 绘制感叹号
            this.ctx.fillStyle = '#FFFF00';
            this.ctx.font = '16px Arial';
            this.ctx.fillText('!', npc.x - 3, npc.y - 25);
        });
    }
    
    drawPlayer() {
        this.ctx.fillStyle = '#FFD700';
        this.ctx.fillRect(this.player.x - this.player.width/2, this.player.y - this.player.height/2, this.player.width, this.player.height);
        
        // 绘制面向
        this.ctx.fillStyle = '#000000';
        if (this.player.facing === 'right') {
            this.ctx.fillRect(this.player.x + 8, this.player.y - 5, 4, 4);
        } else {
            this.ctx.fillRect(this.player.x - 12, this.player.y - 5, 4, 4);
        }
    }
    
    drawParticles() {
        this.particles.forEach(particle => {
            this.ctx.fillStyle = particle.color;
            this.ctx.globalAlpha = particle.life / 30;
            this.ctx.fillRect(particle.x - 2, particle.y - 2, 4, 4);
        });
        this.ctx.globalAlpha = 1;
    }
    
    drawFloatingTexts() {
        this.ctx.font = 'bold 16px Arial';
        this.ctx.fillStyle = '#FFEB3B';
        this.ctx.textAlign = 'center';
        this.ctx.shadowColor = 'rgba(0, 0, 0, 0.8)';
        this.ctx.shadowBlur = 4;
        
        this.floatingTexts.forEach(text => {
            this.ctx.globalAlpha = text.life / 60;
            this.ctx.fillText(text.text, text.x, text.y);
        });
        
        this.ctx.globalAlpha = 1;
        this.ctx.textAlign = 'left';
        this.ctx.shadowBlur = 0;
    }
    
    gameLoop() {
        this.update();
        this.render();
        requestAnimationFrame(() => this.gameLoop());
    }
}

// 启动游戏
const game = new FiveElementsGame();