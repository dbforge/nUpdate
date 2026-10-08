// nUpdate 5 homepage mockup: a few small behaviours, no dependencies.
(() => {
  const root = document.documentElement;
  root.classList.add('js');
  const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  // Colour theme: system (no attribute), light or dark, remembered in this browser.
  const themeButtons = [...document.querySelectorAll('[data-theme-choice]')];
  const showTheme = (choice) => themeButtons.forEach((button) => button.setAttribute('aria-pressed', String(button.dataset.themeChoice === choice)));
  themeButtons.forEach((button) => button.addEventListener('click', () => {
    const choice = button.dataset.themeChoice;
    if (choice === 'system') delete root.dataset.theme;
    else root.dataset.theme = choice;
    try {
      if (choice === 'system') localStorage.removeItem('nupdate-theme');
      else localStorage.setItem('nupdate-theme', choice);
    } catch {
      // Without storage the choice lasts until the page is left.
    }
    showTheme(choice);
  }));
  showTheme(root.dataset.theme ?? 'system');
  const wait = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

  // Reveal sections as they scroll into view.
  const revealed = document.querySelectorAll('.reveal');
  if ('IntersectionObserver' in window && !reducedMotion) {
    const observer = new IntersectionObserver((entries) => {
      for (const entry of entries) {
        if (entry.isIntersecting) {
          entry.target.classList.add('is-visible');
          observer.unobserve(entry.target);
        }
      }
    }, { threshold: 0.15, rootMargin: '0px 0px -40px 0px' });
    revealed.forEach((element) => observer.observe(element));
  } else {
    revealed.forEach((element) => element.classList.add('is-visible'));
  }

  // Copy buttons.
  document.querySelectorAll('[data-copy]').forEach((button) => {
    button.addEventListener('click', async () => {
      const source = document.querySelector(button.dataset.copy);
      try {
        await navigator.clipboard.writeText(source.textContent.trim());
        button.textContent = 'Copied';
        button.classList.add('is-copied');
        setTimeout(() => { button.textContent = 'Copy'; button.classList.remove('is-copied'); }, 1600);
      } catch {
        window.getSelection().selectAllChildren(source);
      }
    });
  });

  // Tabs: code examples and Administration screenshots.
  document.querySelectorAll('[data-tabs]').forEach((group) => {
    const tabs = [...group.querySelectorAll('[role="tab"]')];
    const select = (tab) => {
      for (const other of tabs) {
        const selected = other === tab;
        other.setAttribute('aria-selected', String(selected));
        other.tabIndex = selected ? 0 : -1;
        document.getElementById(other.getAttribute('aria-controls')).hidden = !selected;
      }
    };
    select(tabs.find((tab) => tab.getAttribute('aria-selected') === 'true') ?? tabs[0]);
    tabs.forEach((tab, index) => {
      tab.addEventListener('click', () => select(tab));
      tab.addEventListener('keydown', (event) => {
        const step = { ArrowRight: 1, ArrowDown: 1, ArrowLeft: -1, ArrowUp: -1 }[event.key];
        if (!step) return;
        const next = tabs[(index + step + tabs.length) % tabs.length];
        select(next);
        next.focus();
      });
    });
  });

  // The Administration windows are 920 pixels wide; narrower stages show them scaled down.
  const stage = document.querySelector('.stage');
  if (stage && 'ResizeObserver' in window) {
    new ResizeObserver(() => stage.style.setProperty('--scale', Math.min(1, stage.clientWidth / 920).toFixed(4))).observe(stage);
  }

  // API reference: one page of types; the list jumps to them, highlights the one in view and filters members.
  const api = document.querySelector('.api');
  if (api) {
    const nav = api.querySelector('.api-nav');
    const types = [...api.querySelectorAll('.api-type')];
    const links = new Map([...nav.querySelectorAll('a')].map((link) => [link.hash.slice(1), link]));
    const filter = document.getElementById('api-filter');
    const empty = document.getElementById('api-empty');

    filter.addEventListener('input', () => {
      const query = filter.value.trim().toLowerCase();
      let total = 0;
      for (const type of types) {
        let hits = 0;
        for (const group of type.querySelectorAll('.api-group')) {
          let groupHits = 0;
          for (const member of group.querySelectorAll('.api-member')) {
            const hit = !query || member.dataset.search.includes(query);
            member.hidden = !hit;
            if (hit) groupHits++;
          }
          group.hidden = groupHits === 0;
          hits += groupHits;
        }
        type.hidden = hits === 0;
        links.get(type.id).classList.toggle('is-empty', hits === 0);
        total += hits;
      }
      empty.hidden = total > 0;
    });
    // Jumping to a type ends the filter, so the type is there to jump to.
    links.forEach((link) => link.addEventListener('click', () => {
      if (!filter.value) return;
      filter.value = '';
      filter.dispatchEvent(new Event('input'));
    }));

    // Highlight the type at the top of the screen, and keep it visible in the list.
    const markCurrent = (type) => {
      links.forEach((link, id) => {
        if (id === type.id) link.setAttribute('aria-current', 'true');
        else link.removeAttribute('aria-current');
      });
      const link = links.get(type.id);
      if (getComputedStyle(nav).position === 'sticky'
          && (link.offsetTop < nav.scrollTop || link.offsetTop + link.offsetHeight > nav.scrollTop + nav.clientHeight)) {
        nav.scrollTop = link.offsetTop - nav.clientHeight / 3;
      }
    };
    // The current type is the last one whose top has passed below the sticky header.
    let scheduled = false;
    const update = () => {
      scheduled = false;
      const visible = types.filter((type) => !type.hidden);
      if (visible.length === 0) return;
      const atBottom = window.innerHeight + window.scrollY >= document.documentElement.scrollHeight - 2;
      let current = visible[0];
      for (const type of visible) {
        if (type.getBoundingClientRect().top <= 120) current = type;
      }
      if (atBottom) {
        // The last types cannot reach the top; prefer the one just jumped to, else the last one on screen.
        const onScreen = (type) => type.getBoundingClientRect().top < window.innerHeight;
        const target = document.getElementById(decodeURIComponent(location.hash.slice(1)));
        current = visible.includes(target) && onScreen(target) ? target : visible.findLast(onScreen) ?? current;
      }
      markCurrent(current);
    };
    const schedule = () => {
      if (scheduled) return;
      scheduled = true;
      requestAnimationFrame(update);
    };
    window.addEventListener('scroll', schedule, { passive: true });
    window.addEventListener('resize', schedule);
    filter.addEventListener('input', schedule);
    update();
  }

  // A tiny highlighter for the C# and project-file snippets.
  const csharp = /(\/\/[^\n]*|\/\*[\s\S]*?\*\/)|("(?:[^"\\\n]|\\.)*")|\b(var|new|if|await|static|int|float|string|void|sealed|class|protected|override|using|return|true|false|null)\b|\b(UpdateManager|UpdaterUI|Uri|Path|AppContext|Stability|InstallerHost|InstallerSession|WindowProgressReporter|STAThread|ApplicationVersion)\b/g;
  const xml = /(<!--[\s\S]*?-->)|("[^"]*")|(<\/?[\w.]+|\/?>)|(\s[\w:]+(?==))/g;
  const escape = (text) => text.replace(/[&<>]/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;' }[c]));
  const classes = { cs: ['tok-com', 'tok-str', 'tok-key', 'tok-type'], xml: ['tok-com', 'tok-str', 'tok-tag', 'tok-attr'] };
  document.querySelectorAll('code[data-lang]').forEach((code) => {
    const lang = code.dataset.lang;
    const text = code.textContent;
    let html = '';
    let last = 0;
    for (const match of text.matchAll(lang === 'xml' ? xml : csharp)) {
      const group = match.slice(1).findIndex((part) => part !== undefined);
      html += escape(text.slice(last, match.index)) + `<span class="${classes[lang][group]}">${escape(match[0])}</span>`;
      last = match.index + match[0].length;
    }
    code.innerHTML = html + escape(text.slice(last));
  });

  // Which file of version 2.2.0 a client takes: its runtime identifier, else its OS, else any.
  const files = new Map([...document.querySelectorAll('.files li')].map((item) => [item.dataset.platform, item]));
  const verdict = document.getElementById('verdict');
  const clientButtons = [...document.querySelectorAll('.clients button')];
  let resolveRun = 0;
  const resolve = async (rid) => {
    const run = ++resolveRun;
    const os = rid.split('-')[0];
    const order = [[rid, 'the exact runtime identifier'], [os, `no ${rid} file, so its operating system`], ['any', `no ${rid} or ${os} file, so the one for any platform`]];
    files.forEach((item) => item.classList.remove('is-chosen', 'is-candidate', 'is-out'));
    for (const [platform, reason] of order) {
      const item = files.get(platform);
      if (!reason || !item) {
        if (!reducedMotion) await wait(140);
        continue;
      }
      if (!reducedMotion) {
        item.classList.add('is-candidate');
        await wait(260);
        if (run !== resolveRun) return;
      }
      files.forEach((other) => other.classList.toggle('is-out', other !== item));
      item.classList.remove('is-candidate');
      item.classList.add('is-chosen');
      verdict.innerHTML = `<strong>${rid}</strong> takes <code>${platform}.zip</code>: ${reason}.`;
      return;
    }
    verdict.innerHTML = `<strong>${rid}</strong> finds no file, so version 2.2.0 is not offered to it.`;
  };
  clientButtons.forEach((button) => button.addEventListener('click', () => {
    clientButtons.forEach((other) => other.setAttribute('aria-checked', String(other === button)));
    resolve(button.dataset.rid);
  }));
  if (verdict) resolve('win-x64');

  // The installer window, cycling through the three systems.
  const installer = document.querySelector('.installer');
  if (!installer) return;
  const statusLine = document.getElementById('installer-status');
  const bar = document.getElementById('installer-bar');
  const percent = document.getElementById('installer-percent');
  const ridLabel = document.getElementById('installer-rid');
  const osTabs = [...document.querySelectorAll('.os-switch button')];
  const systems = {
    win: { rid: 'win-x64', files: ['Aurora.exe', 'Aurora.dll', 'Aurora.Sync.dll', 'Aurora.resources.dll'], operation: 'Starting service "AuroraSync"...' },
    linux: { rid: 'linux-arm64', files: ['aurora', 'Aurora.dll', 'Aurora.Sync.dll', 'libSkiaSharp.so'], operation: 'Deleting file "legacy.conf"...' },
    mac: { rid: 'osx-arm64', files: ['Aurora', 'Aurora.dll', 'AppIcon.icns', 'Info.plist'], operation: 'Starting process "migrate"...' },
  };
  const order = ['win', 'linux', 'mac'];
  let current = 'win';
  let demoRun = 0;
  let onScreen = true;

  const show = (status, progress) => {
    statusLine.textContent = status;
    const indeterminate = progress === null;
    bar.classList.toggle('is-indeterminate', indeterminate);
    bar.firstElementChild.style.width = indeterminate ? '' : `${progress}%`;
    percent.textContent = indeterminate ? '' : `${Math.round(progress)} %`;
  };
  const selectSystem = (os) => {
    current = os;
    installer.dataset.os = os;
    ridLabel.textContent = `nUpdate.Installer/${systems[os].rid}/`;
    osTabs.forEach((tab) => tab.setAttribute('aria-selected', String(tab.dataset.os === os)));
  };
  const play = async (os) => {
    const run = ++demoRun;
    const alive = () => run === demoRun;
    selectSystem(os);
    installer.classList.remove('is-leaving');
    installer.classList.add('is-entering');
    const system = systems[os];
    const steps = [
      ['Waiting for Aurora to close...', null, 1500],
      ['Extracting files...', null, 900],
      ...system.files.map((file, index) => [`Copying ${file}...`, ((index + 1) / (system.files.length + 1)) * 100, 650]),
      [system.operation, 100, 1300],
    ];
    for (const [status, progress, ms] of steps) {
      if (!alive()) return;
      show(status, progress);
      await wait(ms);
      while (!onScreen && alive()) await wait(400);
    }
    if (!alive()) return;
    installer.classList.remove('is-entering');
    installer.classList.add('is-leaving');
    await wait(450);
    if (alive()) play(order[(order.indexOf(os) + 1) % order.length]);
  };
  osTabs.forEach((tab) => tab.addEventListener('click', () => {
    if (reducedMotion) selectSystem(tab.dataset.os);
    else play(tab.dataset.os);
  }));
  if ('IntersectionObserver' in window) {
    new IntersectionObserver(([entry]) => { onScreen = entry.isIntersecting && !document.hidden; }).observe(installer);
  }
  document.addEventListener('visibilitychange', () => { onScreen = !document.hidden; });
  if (reducedMotion) {
    selectSystem(current);
  } else {
    play(current);
  }
})();
