/*
Asynchronizer - http://www.tumuski.com/2008/07/asynchronous-looping/
Copyright (c) 2008 Thomas Peri

Permission is hereby granted, free of charge, to any person obtaining a
copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

/* Version History:
2008-08-02a: * Added 'dynamic' as a synonym for -1, 
to indicate dynamic clumping.
2008-08-02:  * Added for_in_() method.
* Added synchronous clumping.
* Added safeguards against simultaneous interfering
loops on the same Asynchronizer.
* Changed constructor parameter to accept an object,
for random-access arguments.
2008-07-27:  * Fixed default value of delay argument.
* Switched to the more permissive Expat License.
2008-07-21:  * First version.
*/

/**
* Models asynchronous operations after traditional 'for' and 'while' loops.
* @constructor
*/
var Asynchronizer = function(param) {

    param = param || {};
    param.timeoutDelay = Math.max(0, param.timeoutDelay || 0);
    param.dynamicClumpDuration = Math.max(1, param.dynamicClumpDuration || 50);

    var paused,
		stack,
		timeout,
		validated = false,

	initialize = function() {
	    if (timeout) {
	        clearTimeout(timeout);
	    }
	    timeout = null;
	    paused = false;
	    stack = [];
	},

	running = function() {
	    return stack.length > 0;
	},

	pause = function() {
	    paused = true;
	},

	resume = function() {
	    // What's not paused can't be resumed.
	    if (paused) {
	        paused = false; // Get right to it.

	        // If once() hasn't nullified timeout, then once() hasn't been called
	        // since the pause began, and so the pause never took effect.
	        // If running() is false, then there's no loop to resume.
	        if (!timeout && running()) {
	            once();
	        }
	    }
	},

	top = function() {
	    // The current loop, the one on the top of the stack.
	    return stack.length ? stack[stack.length - 1] : null;
	},

	finish = function() {
	    // Exit the current asynchronous loop, by first calling its callback,
	    // and then removing it from the stack, returning to the one containing it.
	    stack.pop().callback();
	},

	validate = function() {
	    var loop = top();
	    if (timeout || (paused && running())) {
	        throw 'This Asynchronizer is already in use.';
	    }
	    if (loop && loop.options.clump) {
	        throw 'Asynchronous loops cannot happen inside clumped loops.';
	    }
	},

	once = function() {
	    var time,
			index = 0,
			loop = top(),
			clump = loop.options.clump, // The number of iterations to do in a single dispatch.
			proceed = false;

	    // If there was a timeout, it was waiting to call once(),
	    // so nullify it now that we're inside.
	    timeout = null;

	    // Let it be known that the current loop has begun, so as to
	    // delay all but the first iteration.
	    loop.begun = true;

	    // When this Asynchronizer has been paused, 
	    // this is where the train of timeouts stops.
	    if (!paused) {

	        if (clump === 0) {

	            // This loop is not being clumped, so perform it purely asynchronously.
	            // Its condition has already been tested, so execute the statements.
	            // The unclumped_$ object provides the next() and finish() functions to be
	            // called inside loop.statements().
	            loop.statements(unclumped_$);

	        } else {

	            // If this loop is dynamically clumped, decide how many 
	            // iterations to do in this clump.
	            if (loop.dynamic) {
	                // Mark the beginning of this clump, so that we can know 
	                // how long it took.
	                time = new Date().getTime();
	                // Calculate how long the previous one took, only
	                // if there has been a previous one.
	                if (loop.dynamic.time) {
	                    // Here's what we're doing...
	                    // elapsed = time - loop.dynamic.time;
	                    // speed = loop.dynamic.clump / elapsed;
	                    // loop.dynamic.clump = Math.ceil(param.dynamicClumpDuration * speed);
	                    loop.dynamic.clump = Math.ceil(param.dynamicClumpDuration *
							loop.dynamic.clump / (time - loop.dynamic.time));
	                }
	                // Remember:
	                loop.dynamic.time = time;
	                // Decide:
	                clump = loop.dynamic.clump;
	            }

	            // Perform the clump:
	            while (index < clump &&
	            // (index === 0) guards loop.test(), because 
	            // the first test() is done in next()
						(proceed = (index === 0) || loop.test()) &&
						(proceed = loop.statements(clumped_$))) {
	                index++;
	            }

	            // The state of things now that the clump has exited:
	            // 
	            // The loop is guaranteed to run at least once, because clump
	            // is guaranteed to be at least 1 here.
	            //
	            // Therefore, if the loop exits because index reaches the limit of clump,
	            // then proceed will still be true from the last iteration of the loop.
	            // 
	            // If the loop exits because loop.test() returns false, then proceed will
	            // be false, the return value of loop.test().
	            //
	            // Likewise, if the loop exits because loop.statements() returns false,
	            // the result of calling clumped_$.break_(), proceed will also be false.

	            // So, with that in mind:
	            if (proceed) {
	                next();
	            } else {
	                finish();
	            }
	        }
	    }
	},

	next = function() {
	    // Begin the next asynchronous iteration:  Test the condition, and if
	    // it's true, perform the loop's statements. Otherwise, exit the loop.
	    var loop = top();
	    if (loop.test()) {
	        // Enter the statements block:  If it's the first iteration of this loop,
	        // then do it now.  Otherwise, wait a little while.
	        timeout = setTimeout(once, loop.begun ? param.timeoutDelay : 0);
	    } else {
	        finish();
	    }
	},

	while_ = function(test, statements, callback, options) {
	    // Don't bother validating if for_ just did it.
	    // But after validating, clear it for next time.
	    if (!validated) { validate(); }
	    validated = false;

	    // Each loop defaults to having a clump option set to zero,
	    // indicating that the loop should not be clumped.
	    options = options || {};
	    options.clump = options.clump || 0;

	    // Funnel.
	    if (options.clump < 0 || options.clump == 'dynamic') {
	        options.clump = -1;
	    }

	    // Push an object representing this loop onto the stack.
	    stack.push({
	        begun: false,
	        test: test,
	        statements: statements,
	        callback: callback,
	        dynamic: (options.clump == -1) ? { clump: 1, time: 0} : null,
	        options: options
	    }
		);

	    // Start the top loop on the stack, the one that just got pushed on.
	    next();
	},

	for_ = function(init, test, inc, statements, callback, options) {
	    // Since init comes from outside, we can't trust it not to wreak havoc,
	    // so we need to validate before calling it, instead of relying on while_ to do it.
	    validate();
	    validated = true; // Mark this loop as validated.

	    init();
	    while_(
			test,
			function($) {
			    return statements({
			        continue_: function() {
			            inc();
			            return $.continue_();
			        },
			        break_: $.break_
			    });
			},
			callback,
			options);
	},

	for_in_ = function(obj, statements, callback, options) {
	    var i, key, keys = [];
	    for (key in obj) {
	        keys.push(key);
	    }
	    for_(
			function() { i = 0; },
			function() { return i < keys.length; },
			function() { i++; },

			function($) {
			    return statements($, keys[i]);
			},
			callback,
			options);
	},

	chain_ = function() {
	    var i, fns = arguments;
	    for_(
			function() { i = 0; },
			function() { return i < fns.length - 1; },
			function() { i++; },
			function($) {
			    return fns[i]($);
			},
			function() {
			    fns[i]();
			}
	    // not clumped
		);
	},

	lambda = function(v) {
	    return function() {
	        return v;
	    };
	},

	clumped_$ = { continue_: lambda(true), break_: lambda(false) },
	unclumped_$ = { continue_: next, break_: finish };

    // publicize methods
    this.while_ = while_;
    this.for_ = for_;
    this.for_in_ = for_in_;
    this.chain_ = chain_;
    this.running = running;
    this.pause = pause;
    this.resume = resume;
    this.initialize = initialize;
    this.paused = function() {
        return paused;
    };

    initialize();
};
